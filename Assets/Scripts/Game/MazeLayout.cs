using System.Collections.Generic;
using MazePlace.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace MazePlace.Game
{
    /// <summary>
    /// Maps maze cells to world space and builds level meshes. Columns run along +x, rows along
    /// -z (so North is +z, matching Unity's forward), and levels stack along +y.
    /// </summary>
    public static class MazeLayout
    {
        public const float CellSize = 4f;
        public const float WallHeight = 3f;
        public const float WallThickness = 0.25f;
        public const float LevelSpacing = WallHeight + WallThickness;
        public const float ShaftSize = CellSize * 0.5f;

        public const int WallSubmesh = 0;
        public const int FloorSubmesh = 1;

        public static Vector3 CellCentre(Cell cell) =>
            new Vector3(cell.Column * CellSize, cell.Level * LevelSpacing, -cell.Row * CellSize);

        public static Vector3 CellCentre(MazeSize size, int cell) => CellCentre(size.CellAt(cell));

        public static Bounds Bounds(MazeSize size)
        {
            var min = CellCentre(new Cell(0, size.Rows - 1, 0)) - new Vector3(CellSize, WallThickness, CellSize) / 2;
            var max = CellCentre(new Cell(size.Columns - 1, 0, size.Levels - 1)) + new Vector3(CellSize / 2, WallHeight, CellSize / 2);
            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }

        /// <summary>
        /// Walls are drawn on the East/South side of every cell plus the West/North edge of the
        /// grid, so each wall exists exactly once. A floor is left out where a passage leads down.
        /// </summary>
        public static Mesh BuildLevelMesh(Maze maze, int level)
        {
            var size = maze.Size;
            var mesh = new MeshBuilder(submeshCount: 2);
            var alongX = new Vector3(CellSize - WallThickness, WallHeight, WallThickness);
            var alongZ = new Vector3(WallThickness, WallHeight, CellSize - WallThickness);

            for (var row = 0; row < size.Rows; row++)
            {
                for (var column = 0; column < size.Columns; column++)
                {
                    var cell = new Cell(column, row, level);
                    var index = size.IndexOf(cell);
                    var floorCentre = CellCentre(cell);
                    var wallCentre = floorCentre + Vector3.up * WallHeight / 2;

                    var floorSlab = floorCentre - Vector3.up * WallThickness / 2;
                    if (maze.IsOpen(index, Direction.Down))
                        AddFloorAroundShaft(mesh, floorSlab);
                    else
                        mesh.AddBox(FloorSubmesh, floorSlab, new Vector3(CellSize, WallThickness, CellSize));

                    if (!maze.IsOpen(index, Direction.East))
                        mesh.AddBox(WallSubmesh, wallCentre + Vector3.right * CellSize / 2, alongZ);
                    if (!maze.IsOpen(index, Direction.South))
                        mesh.AddBox(WallSubmesh, wallCentre + Vector3.back * CellSize / 2, alongX);
                    if (column == 0)
                        mesh.AddBox(WallSubmesh, wallCentre + Vector3.left * CellSize / 2, alongZ);
                    if (row == 0)
                        mesh.AddBox(WallSubmesh, wallCentre + Vector3.forward * CellSize / 2, alongX);
                }
            }

            // Walls stop short of the corners and posts fill them, so no two walls overlap and z-fight.
            var post = new Vector3(WallThickness, WallHeight, WallThickness);
            for (var row = 0; row <= size.Rows; row++)
                for (var column = 0; column <= size.Columns; column++)
                    mesh.AddBox(WallSubmesh, CellCentre(new Cell(column, row, level)) + new Vector3(-CellSize / 2, WallHeight / 2, CellSize / 2), post);

            return mesh.Build($"Level {level}");
        }

        /// <summary>
        /// The shaft only takes up the middle of the cell, so the player can walk around it
        /// instead of always dropping through.
        /// </summary>
        private static void AddFloorAroundShaft(MeshBuilder mesh, Vector3 centre)
        {
            var rim = (CellSize - ShaftSize) / 2;
            var offset = (ShaftSize + rim) / 2;
            mesh.AddBox(FloorSubmesh, centre + Vector3.forward * offset, new Vector3(CellSize, WallThickness, rim));
            mesh.AddBox(FloorSubmesh, centre + Vector3.back * offset, new Vector3(CellSize, WallThickness, rim));
            mesh.AddBox(FloorSubmesh, centre + Vector3.left * offset, new Vector3(rim, WallThickness, ShaftSize));
            mesh.AddBox(FloorSubmesh, centre + Vector3.right * offset, new Vector3(rim, WallThickness, ShaftSize));
        }

        /// <summary>Finds the cell whose floor the given point is standing on or above.</summary>
        public static bool TryGetCell(MazeSize size, Vector3 position, out int cell)
        {
            var candidate = new Cell(
                Mathf.RoundToInt(position.x / CellSize),
                Mathf.RoundToInt(-position.z / CellSize),
                Mathf.FloorToInt((position.y + WallThickness) / LevelSpacing));
            cell = size.Contains(candidate) ? size.IndexOf(candidate) : -1;
            return cell >= 0;
        }

        private sealed class MeshBuilder
        {
            private readonly List<Vector3> vertices = new List<Vector3>();
            private readonly List<Vector3> normals = new List<Vector3>();
            private readonly List<int>[] triangles;

            public MeshBuilder(int submeshCount)
            {
                triangles = new List<int>[submeshCount];
                for (var i = 0; i < submeshCount; i++)
                    triangles[i] = new List<int>();
            }

            public void AddBox(int submesh, Vector3 centre, Vector3 size)
            {
                var half = size / 2;
                for (var axis = 0; axis < 3; axis++)
                {
                    var (u, v) = ((axis + 1) % 3, (axis + 2) % 3);
                    foreach (var sign in new[] { -1f, 1f })
                    {
                        var normal = Vector3.zero;
                        normal[axis] = sign;
                        var faceCentre = centre + Vector3.Scale(normal, half);
                        var du = Vector3.zero;
                        du[u] = half[u];
                        var dv = Vector3.zero;
                        dv[v] = half[v];
                        AddQuad(submesh, normal, faceCentre - du - dv, faceCentre + du - dv, faceCentre + du + dv, faceCentre - du + dv);
                    }
                }
            }

            private void AddQuad(int submesh, Vector3 normal, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
            {
                // Unity treats clockwise triangles as front-facing; flip the winding if this
                // perimeter order would face inwards.
                if (Vector3.Dot(Vector3.Cross(b - a, c - a), normal) < 0)
                    (b, d) = (d, b);

                var start = vertices.Count;
                vertices.AddRange(new[] { a, b, c, d });
                normals.AddRange(new[] { normal, normal, normal, normal });
                triangles[submesh].AddRange(new[] { start, start + 1, start + 2, start, start + 2, start + 3 });
            }

            public Mesh Build(string name)
            {
                var mesh = new Mesh { name = name, indexFormat = IndexFormat.UInt32, subMeshCount = triangles.Length };
                mesh.SetVertices(vertices);
                mesh.SetNormals(normals);
                for (var i = 0; i < triangles.Length; i++)
                    mesh.SetTriangles(triangles[i], i);
                mesh.RecalculateBounds();
                return mesh;
            }
        }
    }
}

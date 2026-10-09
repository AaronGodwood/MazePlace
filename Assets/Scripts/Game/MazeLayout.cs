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

            for (var row = 0; row < size.Rows; row++)
            {
                for (var column = 0; column < size.Columns; column++)
                {
                    var cell = new Cell(column, row, level);
                    var index = size.IndexOf(cell);
                    var floorCentre = CellCentre(cell);
                    var wallCentre = floorCentre + Vector3.up * WallHeight / 2;

                    if (!maze.IsOpen(index, Direction.Down))
                        mesh.AddBox(FloorSubmesh, floorCentre - Vector3.up * WallThickness / 2, new Vector3(CellSize, WallThickness, CellSize));

                    if (!maze.IsOpen(index, Direction.East))
                        mesh.AddBox(WallSubmesh, wallCentre + Vector3.right * CellSize / 2, new Vector3(WallThickness, WallHeight, CellSize + WallThickness));
                    if (!maze.IsOpen(index, Direction.South))
                        mesh.AddBox(WallSubmesh, wallCentre + Vector3.back * CellSize / 2, new Vector3(CellSize + WallThickness, WallHeight, WallThickness));
                    if (column == 0)
                        mesh.AddBox(WallSubmesh, wallCentre + Vector3.left * CellSize / 2, new Vector3(WallThickness, WallHeight, CellSize + WallThickness));
                    if (row == 0)
                        mesh.AddBox(WallSubmesh, wallCentre + Vector3.forward * CellSize / 2, new Vector3(CellSize + WallThickness, WallHeight, WallThickness));
                }
            }

            return mesh.Build($"Level {level}");
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

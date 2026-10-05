using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace priority_queue
{
    class PriorityQueue<TElement>
    {
        private List<TElement> elements;
        private List<double> prioritys;
        public int Count { get; set; }
        public PriorityQueue()
        {
            elements = new List<TElement>();
            prioritys = new List<double>();
            Count = 0;
        }
        

        public void Enqueue(TElement element, double priority)
        {
            
            if(elements.Count == 0)
            {
                elements.Add(element);
                prioritys.Add(priority);
                Count++;
            }
            else
            {
                for(int i = 0; i < elements.Count;i++)
                {
                    if(priority < prioritys[i])
                    {
                        prioritys = ShiftUp(prioritys,i);
                        elements = ShiftUp(elements,i);
                        elements[i] = element;
                        prioritys[i] = priority;
                        Count++;
                        break;
                    }
                    else if(i == elements.Count-1)
                    {
                        elements.Add(element);
                        prioritys.Add(priority);
                        Count++;
                        break;
                    }
                }
            }
        }

        private List<T> ShiftUp<T>(List<T> list, int startIndex)
        {
            T element = list[^1];
            for (int i =list.Count-1 ;i > startIndex; i--) // hmmmm
            {
                list[i] = list[i - 1];
            }
            list.Add(element);
            return list;
        }

        private List<T> ShiftDown<T>(List<T> list)
        {
            for(int i = 0; i < list.Count-1; i++)
            {
                list[i] = list[i+1];
            }
            return list;
        }

        public void Clear()
        {
            elements.Clear();
            prioritys.Clear();
        }

        public TElement Dequeue()
        {
            TElement firstElement;
            if (elements.Count != 0)
            {
                firstElement = elements[0];
                Count--;
                elements = ShiftDown<TElement>(elements);
                prioritys = ShiftDown<double>(prioritys);
                elements.Remove(elements[^1]);
                prioritys.Remove(prioritys[^1]);
                return firstElement;
            }
            throw new InvalidOperationException();
            
            
        }

        public void Display()
        {
            for(int i = 0;i < elements.Count;i++)
            {
                Console.WriteLine(elements[i]);
            }
        }



    }
}

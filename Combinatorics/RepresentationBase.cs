﻿using System;

namespace Combinatorics
{
    public abstract class RepresentationBase<T>
    {
        public T[] Elements
        {
            get;
            set;
        }

        public int ElementCount
        {
            get;
            private set;
        }


        public int SelectedElementCount
        {
            get;
            private set;
        }

        public RepresentationBase(int elementCount, int selectedElementCount)
        {
            ElementCount = elementCount;
            SelectedElementCount = selectedElementCount;
        }

        public abstract RepresentationBase<T> GetCopy();

        public override string ToString()
        {
            string elementsInfo = String.Empty;
            foreach (var item in Elements)
            {
                elementsInfo += item.ToString() + " ";
            }
            return elementsInfo;
        }
    }
}

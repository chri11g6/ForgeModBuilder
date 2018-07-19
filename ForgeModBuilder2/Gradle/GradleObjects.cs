﻿using System.Collections.Generic;

namespace ForgeModBuilder.Gradle
{

    public class GObject
    {
        public string Name { get; set; }
        public int NestedLevel { get; set; } = -1;

        public override string ToString()
        {
            return GetTab() + Name;
        }

        public string GetTab()
        {
            string tab = "";
            if (NestedLevel > 0)
            {
                tab = new string(' ', NestedLevel * 4);
            }
            return tab;
        }

        public override bool Equals(object obj)
        {
            if (obj is GObject)
            {
                GObject gobject = (GObject)obj;
                return gobject.Name == this.Name && gobject.NestedLevel == this.NestedLevel;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class GVariable : GObject
    {
        public object Value { get; set; }

        public GVariable(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return GetTab() + Name + " = " + Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is GVariable)
            {
                GVariable gvariable = (GVariable)obj;
                return gvariable.Name == this.Name && gvariable.NestedLevel == this.NestedLevel && gvariable.Value == this.Value;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class GBlock : GObject
    {
        public List<GObject> Children { get; private set; } = new List<GObject>();

        public override string ToString()
        {
            string tab = GetTab();
            string text = tab + Name + " {\n";
            foreach (GObject child in Children)
            {
                text += child + "\n";
            }
            text += tab + "}";
            return text;
        }

        public bool HasChild(string Name)
        {
            foreach (GObject child in Children)
            {
                if (child.Name == Name)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasChild<T>(string Name) where T : GObject
        {
            foreach (GObject child in Children)
            {
                if (child.Name == Name && child is T)
                {
                    return true;
                }
            }
            return false;
        }

        public T SelectChild<T>(string Name) where T : GObject
        {
            foreach (GObject child in Children)
            {
                if (child.Name == Name && child is T)
                {
                    return (T)child;
                }
            }
            return null;
        }

        public List<T> SelectChildren<T>(bool deep = false) where T : GObject
        {
            List<T> selectedChildren = new List<T>();
            foreach (GObject child in Children)
            {
                if (child is T)
                {
                    selectedChildren.Add((T)child);
                }
                if (child is GBlock && deep)
                {
                    selectedChildren.AddRange(((GBlock)child).SelectChildren<T>(deep));
                }
            }
            return selectedChildren;
        }
    }

    public class GTask : GBlock
    {
        public GTask(GBlock block)
        {
            Name = block.Name;
            Children.AddRange(block.Children);
            NestedLevel = block.NestedLevel;
        }

        public override string ToString()
        {
            string tab = GetTab();
            string text = tab + "task " + Name + " {\n";
            foreach (GObject child in Children)
            {
                text += child + "\n";
            }
            text += tab + "}";
            return text;
        }
    }

    public static class GradleExtensions
    {
        public static bool ContainsObject(this List<GObject> list, string name)
        {
            foreach (GObject child in list)
            {
                if (child.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public static GObject GetObject(this List<GObject> list, string name)
        {
            foreach (GObject child in list)
            {
                if (child.Name == name)
                {
                    return child;
                }
            }
            return null;
        }
    }

}

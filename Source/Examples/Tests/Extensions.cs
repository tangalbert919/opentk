﻿#region --- License ---
/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.OpenGL.Enums;
using OpenTK.Graphics;
using System.Text.RegularExpressions;

namespace Examples.WinForms
{
    [Example("Extensions", ExampleCategory.Test)]
    public partial class Extensions : Form
    {
        #region Fields

        int supported_count, opengl_function_count;      // Number of supported extensions.
        SortedDictionary<Function, bool> functions = new SortedDictionary<Function, bool>();

        #endregion

        #region Constructors

        public Extensions()
        {
            this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();

            Application.Idle += StartAsync;
        }

        #endregion

        #region Private Members

        // Creates a context and starts processing the GL class.
        // The processing takes place in the background to avoid hanging the GUI.
        void StartAsync(object sender, EventArgs e)
        {
            Application.Idle -= StartAsync;

            // Create a context in order to load all GL methods (GL.LoadAll() is called automatically.)
            using (GLControl control = new GLControl(GraphicsMode.Default))
            {
                TextBoxVendor.Text = GL.GetString(StringName.Vendor);
                TextBoxRenderer.Text = GL.GetString(StringName.Renderer);
                TextBoxVersion.Text = GL.GetString(StringName.Version);
            }

            backgroundWorker1.RunWorkerAsync();
        }

        void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (Function f in LoadFunctionsFromType(typeof(GL)))
            {
                // Only show a function as supported when all relevant overloads are supported.
                if (!functions.ContainsKey(f))
                    functions.Add(f, GL.SupportsFunction(f.EntryPoint));
                else
                    functions[f] &= GL.SupportsFunction(f.EntryPoint);
            }

            // Count supported functions using the delegates directly.
            foreach (FieldInfo f in typeof(GL).GetNestedType("Delegates", BindingFlags.NonPublic)
                .GetFields(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (f.GetValue(null) != null)
                    supported_count++;

                opengl_function_count++;
            }
        }

        // Recursively load all functions marked with [AutoGenerated] in the specified Type.
        IEnumerable<Function> LoadFunctionsFromType(Type type)
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                // Functions in GLHelper.cs are not autogenerated and should be skipped.
                AutoGeneratedAttribute[] attr = (AutoGeneratedAttribute[])
                    method.GetCustomAttributes(typeof(AutoGeneratedAttribute), false);
                if (attr.Length == 0)
                    continue;

                yield return new Function(method.Name, type.Name,
                    attr[0].EntryPoint, attr[0].Version, attr[0].Category);
            }

            foreach (Type nested_type in type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
                foreach (Function f in LoadFunctionsFromType(nested_type))
                    yield return f;
        }

        // Update the DataGridView with our findings.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TextBoxSupport.Text = String.Format("{0} of {1} functions supported.",
                supported_count, opengl_function_count);

            foreach (Function f in functions.Keys)
            {
                dataGridView1.Rows.Add(functions[f], f.Name, f.Category, f.Version, f.Extension);
                int index = dataGridView1.Rows.Count - 1;

                // Some simple coloring to make the GridView easier on the eyes.
                // Supported functions are green, unsupported are redish.
                dataGridView1.Rows[index].DefaultCellStyle.BackColor =
                    functions[f] ?
                    (index % 2 != 0 ? Color.FromArgb(128, 255, 192) : Color.FromArgb(192, 255, 192)) :
                    (index % 2 != 0 ? Color.FromArgb(255, 192, 160) : Color.FromArgb(255, 200, 160));
            }

            // Change the width of our Form to make every DataGridView column visible.
            dataGridView1.AutoResizeColumns();
            this.Size = dataGridView1.GetPreferredSize(new Size(2000, Height));
        }

        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (Extensions example = new Extensions())
            {
                Utilities.SetWindowTitle(example);
                example.ShowDialog();
            }
        }

        #endregion
    }

    #region class Function

    // A simple class where we store information from OpenTK.Graphics.GL.
    sealed class Function : IEquatable<Function>, IComparable<Function>
    {
        #region Fields

        // We use these fields to distinguish between functions.
        public readonly string Name;
        public readonly string Category;
        // These fields just provide some extra (cosmetic) information.
        public readonly string EntryPoint;
        public readonly string Version;
        public readonly string Extension;

        #endregion

        #region Constructors

        public Function(string name, string category, string entryPoint, string version, string extension)
        {
            Name = name;
            Category = category == "GL" ? String.Empty : category;
            EntryPoint = entryPoint;
            Version = version;
            Extension = extension;
        }

        #endregion

        #region Public Members

        public override bool Equals(object obj)
        {
            if (obj is Function)
                return this.Equals((Function)obj);

            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Category.GetHashCode();
        }

        #endregion

        #region IEquatable<Function> Members

        public bool Equals(Function other)
        {
            return
                Category == other.Category &&
                Name == other.Name;
        }

        #endregion

        #region IComparable<Function> Members

        public int CompareTo(Function other)
        {
            int order = Category.CompareTo(other.Category);
            if (order == 0)
                order = Name.CompareTo(other.Name);

            return order;
        }

        #endregion
    }

    #endregion
}
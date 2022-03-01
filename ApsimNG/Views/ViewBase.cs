﻿namespace UserInterface.Views
{
    using global::UserInterface.Interfaces;
    using Gtk;
    using System;
    using System.IO;
    using System.Reflection;
    using Utility;

    public class ViewBase : IDisposable
    {
        /// <summary>A builder instance for extracting controls from resource.</summary>
        private Builder builder;

        private string gladeString;

        private object lockObject = new object();

        /// <summary>
        /// A reference to the main view.
        /// </summary>
        public static IMainView MasterView = null;

        /// <summary>
        /// The parent view.
        /// </summary>
        protected ViewBase owner = null;

        /// <summary>
        /// The main widget in this view.
        /// </summary>
        protected Widget mainWidget = null;

        /// <summary>
        /// Used to detect redundant calls to Dispose
        /// This is standard fare in IDisposable implementations
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Displays an error message to the user.
        /// </summary>
        /// <param name="err"></param>
        protected void ShowError(Exception err)
        {
            MasterView.ShowError(err);
        }

        /// <summary>
        /// Asks the user for a file or directory. If you need more specialised behaviour 
        /// (e.g. select multiple files), you will need to instantiate and use an 
        /// implementation of <see cref="IFileDialog"/>.
        /// </summary>
        /// <param name="prompt">Prompt to be displayed in the title bar of the dialog.</param>
        /// <param name="actionType">Type of action the dialog should perform.</param>
        /// /// <param name="fileType">File types the user is allowed to choose.</param>
        /// <param name="initialDirectory">Initial directory. Defaults to the previously used directory.</param>
        /// <returns>Path to the chosen file or directory.</returns>
        public static string AskUserForFileName(string prompt, FileDialog.FileActionType actionType, string fileType, string initialDirectory = "")
        {
            IFileDialog dialog = new FileDialog()
            {
                Prompt = prompt,
                Action = actionType,
                FileType = fileType,
                InitialDirectory = initialDirectory
            };
            return dialog.GetFile();
        }

        /// <summary>
        /// Returns a new Builder object generated by parsing the glade 
        /// text found in the indicated resource.
        /// </summary>
        /// <param name="resourceName">Name of the resouce.</param>
        /// <returns>A new Builder object, or null on failure.</returns>
        public static Builder BuilderFromResource(string resourceName)
        {
            Stream resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (resStream == null)
                return null;
            using (StreamReader reader = new StreamReader(resStream))
            {
                var gladeString = reader.ReadToEnd();

                Builder result = new Builder();
                result.AddFromString(gladeString);
                return result;
            }
        }

        /// <summary>
        /// Returns a new Builder object generated by parsing the glade 
        /// text found in the indicated resource.
        /// </summary>
        /// <param name="resourceName">Name of the resouce.</param>
        /// <returns>A new Builder object, or null on failure.</returns>
        public Builder GetBuilderFromResource(string resourceName)
        {
            Stream resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (resStream == null)
                return null;
            using (StreamReader reader = new StreamReader(resStream))
            {
                gladeString = reader.ReadToEnd();

                Builder result = new Builder();
                result.AddFromString(gladeString);
                return result;
            }
        }

        private void SetMainWidget()
        {
            // Find the top level control id.
            int posFirstID = gladeString.IndexOf("id=\"");
            if (posFirstID != -1)
            {
                posFirstID += "id=\"".Length;
                var posCloseQuote = gladeString.IndexOf("\"", posFirstID);
                var controlID = gladeString.Substring(posFirstID, posCloseQuote - posFirstID);
                mainWidget = (Gtk.Widget)builder.GetObject(controlID);
            }
        }

        /// <summary>
        /// The parent view.
        /// </summary>
        public ViewBase Owner
        {
            get
            {
                return owner;
            }
        }

        /// <summary>
        /// The main widget in this view.
        /// </summary>
        public Widget MainWidget
        {
            get
            {
                return mainWidget;
            }
        }

        /// <summary>.</summary>
        public ViewBase()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner">The parent view.</param>
        public ViewBase(ViewBase owner)
        {
            this.owner = owner;
        }

        public ViewBase(ViewBase owner, string gladeResourceName)
        {
            this.owner = owner;
            SetGladeResource(gladeResourceName);
        }

        /// <summary>
        /// Set the GLADE resource to use.
        /// </summary>
        /// <param name="gladeResourceName">The GLADE resource name.</param>
        public void SetGladeResource(string gladeResourceName)
        {
            builder = GetBuilderFromResource(gladeResourceName);
            SetMainWidget();
        }

        /// <summary>
        /// Get a control on the view.
        /// </summary>
        /// <typeparam name="T">The type of the control.</typeparam>
        /// <param name="controlName">The name of the control.</param>
        /// <returns>The control or null if not found.</returns>
        public T GetControl<T>(string controlName) where T : ViewBase, new()
        {
            T control = new T();
            control.Initialise(this, builder.GetObject(controlName));
            return control;
        }

        /// <summary>
        /// Invoke an event handler on the main application thread.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        public void InvokeOnMainThread(EventHandler handler)
        {
            if (handler != null)
            {
                // The invoke below exits immediately before the handler has completed
                // running. This can be problem if InvokeOnMainThread is called again
                // before the previous call has finished running.
                lock (lockObject)
                    Application.Invoke(handler);
            }
        }

        /// <summary>
        /// A method used when a view is wrapping a gtk control.
        /// </summary>
        /// <param name="ownerView">The owning view.</param>
        /// <param name="gtkControl">The gtk control being wrapped.</param>
        protected virtual void Initialise(ViewBase ownerView, GLib.Object gtkControl)
        {
            owner = ownerView;
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing && mainWidget != null)
                {
                    Utility.GtkUtil.DetachAllHandlers(mainWidget);
                    bool isToplevel = mainWidget.IsToplevel;
                    mainWidget.Destroy();
                    // I'm not sure whether this is necessary, but I was getting rather rare, unpredicatable
                    // access violations as described in https://githubmemory.com/repo/GtkSharp/GtkSharp/issues/248
                    // The suggestion there was to Dispose the Widget, at least if it's top-level
                    // CAUTION: Behaviour is likely to change with new versions of Gtk#
                    //if (isToplevel)
                     mainWidget.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.

                // Note disposing has been done.
                disposed = true;
            }
        }

    }
}
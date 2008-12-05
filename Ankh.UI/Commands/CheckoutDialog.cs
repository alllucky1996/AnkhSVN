using System;
using System.ComponentModel;
using System.Windows.Forms;

using SharpSvn;
using Ankh.UI.RepositoryExplorer;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog for performing checkouts.
    /// </summary>
    public partial class CheckoutDialog : System.Windows.Forms.Form
    {
        public CheckoutDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.ControlsChanged(this, EventArgs.Empty);
            revisionPicker.Revision = SvnRevision.Head;
        }

        /// <summary>
        /// The URL of the repository.
        /// </summary>
        public Uri Uri
        {
            get
            {
                Uri uri;
                if (string.IsNullOrEmpty(urlTextBox.Text) || !Uri.TryCreate(urlTextBox.Text, UriKind.Absolute, out uri))
                    return null;

                return uri;
            }
            set 
            {
                if (value == null)
                    urlTextBox.Text = "";
                else
                    urlTextBox.Text = value.AbsoluteUri;
            }
        }

        IAnkhServiceProvider _context;
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; this.revisionPicker.Context = value; }
        }

        /// <summary>
        /// The local path to check out to.
        /// </summary>
        public string LocalPath
        {
            get { return this.localDirTextBox.Text; }
            set { this.localDirTextBox.Text = value; }
        }

        /// <summary>
        /// The revision to check out.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SvnRevision Revision
        {
            get { return this.revisionPicker.Revision; }
        }

        /// <summary>
        /// Whether to perform a non-recursive checkout.
        /// </summary>
        public bool Recursive
        {
            get { return !this.nonRecursiveCheckBox.Checked; }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }


        /// <summary>
        /// Validate the input here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlsChanged(object sender, System.EventArgs e)
        {
            this.okButton.Enabled = this.revisionPicker.Valid && Uri != null && !string.IsNullOrEmpty(localDirTextBox.Text);
        }

        /// <summary>
        /// Let the user browse for a directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseClicked(object sender, System.EventArgs e)
        {
			using (FolderBrowserDialog browser = new FolderBrowserDialog())
            {
                if (browser.ShowDialog(this) == DialogResult.OK)
                {
                    this.localDirTextBox.Text = browser.SelectedPath;
                }
            }
        }

        private void urlTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Uri != null)
                revisionPicker.SvnOrigin = new Ankh.Scc.SvnOrigin(Uri, Uri);
        }

        private void urlBrowse_Click(object sender, EventArgs e)
        {
            using (RepositoryFolderBrowserDialog dlg = new RepositoryFolderBrowserDialog())
            {
                dlg.Context = Context;
                dlg.SelectedUri = Uri;

                if (dlg.ShowDialog(Context) == DialogResult.OK)
                    Uri = dlg.SelectedUri;
            }
        }
    }
}

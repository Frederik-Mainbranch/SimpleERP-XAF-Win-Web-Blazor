using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auftragserfassung_Blazor.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.XtraGrid.Views.Grid;
using System.Drawing;


namespace Auftragserfassung_Blazor.Module.Win.Controllers
{
    public partial class NestedListviewShowColumn_Bestellung : ViewController<ListView>
    {
        public NestedListviewShowColumn_Bestellung()
        {
            InitializeComponent();
            TargetViewNesting = Nesting.Nested;
            TargetObjectType = typeof(BestellungsPosition);
        }
        private void UpdateMasterObject(object masterObject)
        {
            Bestellung bestellung = masterObject as Bestellung;
            if (bestellung != null)
            {
                GridListEditor listEditor = ((ListView)View).Editor as GridListEditor;
                if (listEditor != null)
                {
                    GridView gridView = listEditor.GridView;
                    if (gridView != null)
                    {
                        int anzahl = gridView.Columns.Count;
                        gridView.Columns["AnzahlGeliefert"].VisibleIndex = bestellung.BelegWurdeCommitted ? anzahl + 1 : -1;
                        gridView.Columns["AnzahlOffeneRestmenge"].VisibleIndex = bestellung.BelegWurdeCommitted ? anzahl + 2 : -1;
                    }
                }
            }
        }

        private void OnMasterObjectChanged(object sender, System.EventArgs e)
        {
            UpdateMasterObject(((PropertyCollectionSource)sender).MasterObject);
        }
        private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            if (e.PropertyName == "BelegWurdeCommitted")
            {
                UpdateMasterObject(((PropertyCollectionSource)View.CollectionSource).MasterObject);
            };
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            UpdateMasterObject(((PropertyCollectionSource)View.CollectionSource).MasterObject);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.CollectionSource is PropertyCollectionSource)
            {
                PropertyCollectionSource collectionSource = (PropertyCollectionSource)View.CollectionSource;
                ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
                collectionSource.MasterObjectChanged += OnMasterObjectChanged;
            }
        }
        protected override void OnDeactivated()
        {
            if (View.CollectionSource is PropertyCollectionSource)
            {
                PropertyCollectionSource collectionSource = (PropertyCollectionSource)View.CollectionSource;
                ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
                collectionSource.MasterObjectChanged -= OnMasterObjectChanged;
            }
            base.OnDeactivated();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Auftragserfassung_Blazor.Module.BusinessObjects;
using Auftragserfassung_Blazor.Module.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraEditors;

namespace Auftragserfassung_Blazor.Module.Controllers
{
    //public partial class Artikel_Zu_Artikeluntergruppe_hinzufügen : ViewController
    //{
    //    public Artikel_Zu_Artikeluntergruppe_hinzufügen()
    //    {
    //        InitializeComponent();
    //        TargetObjectType = typeof(ArtikelUntergruppe);
    //        TargetViewType = ViewType.DetailView;
    //        PopupWindowShowAction showListViewAction = new PopupWindowShowAction(this, "Artikel zur Artikeluntergruppe hinzufügen", PredefinedCategory.Edit);
    //        showListViewAction.CustomizePopupWindowParams += ShowListViewAction_CustomizePopupWindowParams;
    //        showListViewAction.Execute += Action_Artikel_Zu_Artikeluntergruppe_hinzufügen_execude;
    //    }


    //    private void ShowListViewAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
    //    {
    //        IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Artikel));
    //        CriteriaOperator criteria = new NullOperator(nameof(ArtikelUntergruppe));
    //        string noteListViewId = Application.FindLookupListViewId(typeof(Artikel));
    //        CollectionSourceBase collectionSource = Application.CreateCollectionSource(objectSpace, typeof(Artikel), noteListViewId);
    //        collectionSource.SetCriteria("ArtikelUntergruppe", "ArtikelUntergruppe IS NULL");

    //        e.View = Application.CreateListView(noteListViewId, collectionSource, false);  
    //    }


    //    private void Action_Artikel_Zu_Artikeluntergruppe_hinzufügen_execude(object sender, PopupWindowShowActionExecuteEventArgs e)
    //    {
    //        foreach (Artikel artikelAusgewählt in e.PopupWindowView.SelectedObjects)
    //        {
    //            Artikel artikel = (Artikel)((XPObjectSpace)this.ObjectSpace).Session.GetObjectByKey(artikelAusgewählt.GetType(), artikelAusgewählt.Oid);
    //            artikel.ArtikelUntergruppe = (ArtikelUntergruppe)e.CurrentObject;
    //        }
    //    }
    //}
}

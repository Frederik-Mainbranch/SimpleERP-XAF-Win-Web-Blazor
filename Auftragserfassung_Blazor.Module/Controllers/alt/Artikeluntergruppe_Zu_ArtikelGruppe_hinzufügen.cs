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
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Auftragserfassung_Blazor.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    //public partial class Artikeluntergruppe_Zu_ArtikelGruppe_hinzufügen : ViewController
    //{
    //    public Artikeluntergruppe_Zu_ArtikelGruppe_hinzufügen()
    //    {
    //        InitializeComponent();
    //        TargetObjectType = typeof(Artikelgruppe);
    //        TargetViewType = ViewType.DetailView;
    //        PopupWindowShowAction showListViewAction = new PopupWindowShowAction(this, "Artikeluntergruppe zu Artikelgruppe hinzufügen", PredefinedCategory.Edit);
    //        showListViewAction.CustomizePopupWindowParams += ShowListViewAction_CustomizePopupWindowParams;
    //        showListViewAction.Execute += Action_Artikeluntergruppe_Zu_Artikelgruppe_hinzufügen_execude;
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


    //    private void Action_Artikeluntergruppe_Zu_Artikelgruppe_hinzufügen_execude(object sender, PopupWindowShowActionExecuteEventArgs e)
    //    {
    //        foreach (Artikel artikelAusgewählt in e.PopupWindowView.SelectedObjects)
    //        {
    //            Artikel artikel = (Artikel)((XPObjectSpace)this.ObjectSpace).Session.GetObjectByKey(artikelAusgewählt.GetType(), artikelAusgewählt.Oid);
    //            artikel.ArtikelUntergruppe = (ArtikelUntergruppe)e.CurrentObject;
    //        }

    //    }
    //}
}

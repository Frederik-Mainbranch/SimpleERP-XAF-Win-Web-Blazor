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
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Auftragserfassung_Blazor.Module.Controllers
{
    //// For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    //public partial class PreiseUpdaten : ViewController
    //{
    //  //  SimpleAction updatePreise;
    //    public PreiseUpdaten()
    //    {
    //        InitializeComponent();
    //        TargetObjectType = typeof(BestellungsPosition);

    //        updatePreise = new SimpleAction(this, "Update die Position", PredefinedCategory.Edit);
    //        updatePreise.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
    //        updatePreise.Execute += updatePreise_Execute;
    //    }
    //    protected override void OnActivated()
    //    {
    //        base.OnActivated();
    //        // Perform various tasks depending on the target View.
    //    }
    //    protected override void OnViewControlsCreated()
    //    {
    //        base.OnViewControlsCreated();
    //        // Access and customize the target View control.
    //    }
    //    protected override void OnDeactivated()
    //    {
    //        // Unsubscribe from previously subscribed events and release other references and resources.
    //        base.OnDeactivated();
    //    }

    //    private void updatePreise_Execute(object sender, SimpleActionExecuteEventArgs e)
    //    {
    //        var currentObj = e.CurrentObject as BestellungsPosition;
    //        if(currentObj != null)
    //        {
    //            currentObj.BestellterArtikelPreis = currentObj.Artikel.Preis;
    //            currentObj.BestellterArtikelBezeichnung = currentObj.Artikel.Bezeichnung;
    //        }

    //        if(this.ObjectSpace.IsModified)
    //        {
    //            this.ObjectSpace.CommitChanges();
    //        }
    //    }
    //}
}

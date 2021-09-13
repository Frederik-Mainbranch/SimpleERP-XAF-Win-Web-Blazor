using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DisplayNameAttribute = DevExpress.Xpo.DisplayNameAttribute;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Versteckt")]



    public abstract class Firma : PersonenKonto
    {
        public Firma(Session session)
            : base(session)
        {
        }

        //---------------------------- Klasse ------------------------------------
        //---------------------------- Override Methoden -------------------------------



        //---------------------------- Override Methode -------------------------------
        //-------------------------------- Properties ---------------------------------------------


        //-------------------------------- Properties ---------------------------------------------
        //-------------------------------- Listen ---------------------------------------------

        //-------------------------------- Listen ---------------------------------------------
        //-------------------------------- Non Persistent Properties ---------------------------------------------


        //-------------------------------- Non Persistent Properties ---------------------------------------------



    }
}
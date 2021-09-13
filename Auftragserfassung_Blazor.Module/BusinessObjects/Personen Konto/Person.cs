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

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty("VolleAnrede")]
    [NavigationItem("Versteckt")]

    public abstract class Person : PersonenKonto
    {
        public Person(Session session)
            : base(session)
        {
        }


        //---------------------------- Klasse ------------------------------------
        //---------------------------- Override Methoden -------------------------------

        //---------------------------- Override Methode -------------------------------
        //-------------------------------- Properties ---------------------------------------------


        private string _Titel;
        public string Titel
        {
            get { return _Titel; }
            set { SetPropertyValue<string>(nameof(Titel), ref _Titel, value); }
        }



        private string _Vorname;
        public string Vorname
        {
            get { return _Vorname; }
            set { SetPropertyValue<string>(nameof(Vorname), ref _Vorname, value); }
        }


        //-------------------------------- Properties ---------------------------------------------
        //-------------------------------- Listen ---------------------------------------------

        //-------------------------------- Listen ---------------------------------------------
        //-------------------------------- Non Persistent Properties ---------------------------------------------


        [DevExpress.Xpo.DisplayNameAttribute("VolleAnrede")]
        [VisibleInDetailView(false)]
        public string VolleAnrede
        {
            get { return Titel + " " + Vorname + " " + Name; }
        }



        //-------------------------------- Non Persistent Properties ---------------------------------------------



    }


}
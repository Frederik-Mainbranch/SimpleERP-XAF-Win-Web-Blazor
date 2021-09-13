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
using DevExpress.ExpressApp.ConditionalAppearance;
using DisplayNameAttribute = DevExpress.Xpo.DisplayNameAttribute;

namespace Auftragserfassung_Blazor.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Versteckt")]


    public abstract class Hybrid : PersonenKonto
    {
        public Hybrid(Session session)
            : base(session)
        {
        }


        //---------------------------- Klasse ------------------------------------
        //---------------------------- Override Methoden -------------------------------

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (propertyName == nameof(KontoIstEineFirma))
            {
                if (KontoIstEineFirma == true)
                {
                    Vorname = "";
                    Titel = "Firma";
                }
                else
                {
                    Titel = "";
                }
            }
        }


        //---------------------------- Override Methode -------------------------------
        //-------------------------------- Properties ---------------------------------------------


        private string _Titel;
        [Appearance("TitelSperren", TargetItems = "Titel", Criteria = "KontoIstEineFirma == true", Enabled = false)]
        public string Titel
        {
            get { return _Titel; }
            set { SetPropertyValue<string>(nameof(Titel), ref _Titel, value); }
        }


        private string _Vorname;
        [Appearance("Verstecke Vorname", TargetItems = "Vorname", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.ShowEmptySpace, Criteria = "KontoIstEineFirma == true")]
        [Appearance("Grau Vorname", TargetItems = "Vorname", BackColor = "#f2f2f2", Criteria = "KontoIstEineFirma == true")]
        public string Vorname
        {
            get { return _Vorname; }
            set { SetPropertyValue<string>(nameof(Vorname), ref _Vorname, value); }
        }


        private bool _KontoIstEineFirma;
        [DisplayNameAttribute("Firma")]
        [ImmediatePostData]
        public bool KontoIstEineFirma
        {
            get { return _KontoIstEineFirma; }
            set { SetPropertyValue<bool>(nameof(KontoIstEineFirma), ref _KontoIstEineFirma, value); }
        }


        //-------------------------------- Properties ---------------------------------------------
        //-------------------------------- Listen ---------------------------------------------

        //-------------------------------- Listen ---------------------------------------------
        //-------------------------------- Non Persistent Properties ---------------------------------------------


        //-------------------------------- Non Persistent Properties ---------------------------------------------
    }
    }
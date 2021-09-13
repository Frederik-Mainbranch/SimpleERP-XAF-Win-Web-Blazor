using Auftragserfassung_Blazor.Module.BusinessObjects;
using Auftragserfassung_Blazor.Module.Interfaces;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auftragserfassung_Blazor.Module.Helpers
{
    public static class ZeitraumHelper
    {
        public static void ÜberprüfeDatumseingabe(this IZeitraumUeberpruefung zup, Guid oidDerVorübergehendenSteuer)
        {

            DateTime dummyDatime = DateTime.Parse("01.01.0001 00:00:00");

            int eigenePosition = 0;
            bool EintragGefunden = false;
            string pufferBenutzerEingabeStart = (zup.BenutzerEingabeAktionsstart.ToString()).Substring(0, 10);
            string pufferBenutzerEingabeEnde = (zup.BenutzerEingabeAktionsende.ToString()).Substring(0, 10);

            
            IEnumerable<IZeitraumUeberpruefung> andereZups = null;

            switch(zup.GetType().Name)
            {
                case "Aktionspreis":
                    andereZups = zup.AktionsArtikel.AktionspreiseListe.ToList().Cast<IZeitraumUeberpruefung>();
                    break;

                case "vorübergehendeSteuer":
                    andereZups = zup.Steuer.VoruebergehendeSteuerListe.ToList().Cast<IZeitraumUeberpruefung>();
                    for (int i = 0; i < andereZups.Count(); i++)
                    {
                        if (oidDerVorübergehendenSteuer == ((BaseObject)andereZups.ElementAt(i)).Oid)
                        {
                            eigenePosition = i;
                            break;
                        }
                    }
                    break;

                case "Aktionsrabatt":
                    andereZups = zup.AktionsArtikel.AktionsrabatteListe.ToList().Cast<IZeitraumUeberpruefung>();
                    break;
            }



            //Überprüfung, ob die Eingabe in einen Aktionszeitraum liegt
            int counter = 0;
            foreach (IZeitraumUeberpruefung aktPreis in andereZups)
            {
                if(eigenePosition == counter)
                {
                    continue;
                }

                string pufferStart = (aktPreis.AktionStart.ToString()).Substring(0, 10);
                string pufferEnde = (aktPreis.AktionEnde.ToString()).Substring(0, 10);

                //liegt Start/Ende in einen Zeitraum?
                //Start
                if ((zup.BenutzerEingabeAktionsstart >= aktPreis.AktionStart) && (zup.BenutzerEingabeAktionsstart <= aktPreis.AktionEnde))
                {
                    //zup.BenutzerEingabeAktionsstart = dummyDatime;
                    //zup.BenutzerEingabeAktionsende = dummyDatime;
                    EintragGefunden = true;
                    throw new UserFriendlyException("Fehler: Der Aktionsstart liegt in der bereits vorhandenen Aktion vom " + pufferStart + " bis " + pufferEnde + "!");
                }
                //Ende
                else if ((zup.BenutzerEingabeAktionsende >= aktPreis.AktionStart) && (zup.BenutzerEingabeAktionsende <= aktPreis.AktionEnde))
                {
                    //zup.BenutzerEingabeAktionsstart = dummyDatime;
                    //zup.BenutzerEingabeAktionsende = dummyDatime;
                    EintragGefunden = true;
                    throw new UserFriendlyException("Fehler: Das Aktionsende liegt in der bereits vorhandenen Aktion vom " + pufferStart + " bis " + pufferEnde + "!");
                }
                //wird ein Aktionszeitraum "umschlossen"?
                else if ((zup.BenutzerEingabeAktionsstart <= aktPreis.AktionStart) && (zup.BenutzerEingabeAktionsende >= aktPreis.AktionEnde))
                {
                    //zup.BenutzerEingabeAktionsstart = dummyDatime;
                    //zup.BenutzerEingabeAktionsende = dummyDatime;
                    EintragGefunden = true;
                    throw new UserFriendlyException("Fehler: Es wird die bereits vorhandene Aktion vom " + pufferStart + " bis " + pufferEnde + " eingeschlossen!");
                }
                //liegt das Ende vor dem Anfang?
                else if (zup.BenutzerEingabeAktionsstart > zup.BenutzerEingabeAktionsende)
                {
                    //zup.BenutzerEingabeAktionsstart = dummyDatime;
                    //zup.BenutzerEingabeAktionsende = dummyDatime;
                    EintragGefunden = true;
                    throw new UserFriendlyException("Fehler: Das Aktionsende " + pufferBenutzerEingabeEnde + " liegt vor dem Aktionsanfang " + pufferBenutzerEingabeStart + "!");
                }
                counter++;
            }


            //Wenn die User Eingaben keinen anderen Aktionszeitraum verletzen
            if (EintragGefunden == false)
            {
                zup.AktionStart = zup.BenutzerEingabeAktionsstart;
                zup.AktionEnde = zup.BenutzerEingabeAktionsende;
            }
        }
    }
}

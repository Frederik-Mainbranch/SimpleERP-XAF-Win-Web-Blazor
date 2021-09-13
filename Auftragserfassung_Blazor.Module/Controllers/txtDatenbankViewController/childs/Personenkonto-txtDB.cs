using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using Auftragserfassung_Blazor.Module.BusinessObjects;
using DevExpress.Xpo.DB;

namespace Auftragserfassung_Blazor.Module.Controllers
{
    public class Personenkonto_txtDB : txtDatenbankViewController
    {
        public Personenkonto_txtDB()
        {
            TargetObjectType = typeof(PersonenKonto);
        }

        protected override void SetzteZielObject(Session session, int anzahlAnzulegenderKunden, Type type)
        {
            int maxKundennummer;
            SelectedData Maxnummer = session.ExecuteQuery("SELECT MAX(Kundennummer) FROM Kunde");
            if ((Maxnummer.ResultSet[0].Rows[0].Values[0] != null))
            {
                maxKundennummer = int.Parse(Maxnummer.ResultSet[0].Rows[0].Values[0].ToString()) + 1;
            }
            else
            {
                maxKundennummer = 1;
            }

            int maxLieferantennummer;
            SelectedData MaxnummerL = session.ExecuteQuery("SELECT MAX(Lieferantennummer) FROM Lieferant");
            if ((MaxnummerL.ResultSet[0].Rows[0].Values[0] != null))
            {
                maxLieferantennummer = int.Parse(MaxnummerL.ResultSet[0].Rows[0].Values[0].ToString()) + 1;
            }
            else
            {
                maxLieferantennummer = 1;
            }

            for (int neuerKundeLaufnummer = 0; neuerKundeLaufnummer < anzahlAnzulegenderKunden; neuerKundeLaufnummer++)
            {
                var personenKonto = (PersonenKonto)Activator.CreateInstance(type, session);
                personenKonto.BearbeitungDurchViewController = true;

                //-------------------------------------------- child spezifisch --------------------------------------------------

                if (type.IsSubclassOf(typeof(Person)) == true)
                {
                    int geschlecht = zufallsWertFeld.Next(0, 2);

                    if (geschlecht == 0)
                    {
                        ((Person)personenKonto).Titel = "Herr";
                        ((Person)personenKonto).Vorname = ZufälligerWertString(Properties.Resources.vorname_maennlich);
                        personenKonto.Name = ZufälligerWertString(Properties.Resources.nachnamen);
                    }
                    else if (geschlecht == 1)
                    {
                        ((Person)personenKonto).Titel = "Frau";
                        ((Person)personenKonto).Vorname = ZufälligerWertString(Properties.Resources.vorname_weiblich);
                        personenKonto.Name = ZufälligerWertString(Properties.Resources.nachnamen);
                    }

                }
                else if (type.IsSubclassOf(typeof(Hybrid)) == true)
                {
                    int geschlecht = zufallsWertFeld.Next(0, 3);

                    if (geschlecht == 0)
                    {
                        ((Hybrid)personenKonto).Titel = "Herr";
                        ((Hybrid)personenKonto).Vorname = ZufälligerWertString(Properties.Resources.vorname_maennlich);
                        personenKonto.Name = ZufälligerWertString(Properties.Resources.nachnamen);
                    }
                    else if (geschlecht == 1)
                    {
                        ((Hybrid)personenKonto).Titel = "Frau";
                        ((Hybrid)personenKonto).Vorname = ZufälligerWertString(Properties.Resources.vorname_weiblich);
                        personenKonto.Name = ZufälligerWertString(Properties.Resources.nachnamen);
                    }
                    else if (geschlecht == 2)
                    {
                        ((Hybrid)personenKonto).KontoIstEineFirma = true;
                        personenKonto.Name = ZufälligerWertString(Properties.Resources.nachnamen) + " " + ZufälligerWertString(Properties.Resources.firmenRechtsformen);
                    }

                    if(personenKonto.GetType() == typeof(Kunde))
                    {
                        ((Kunde)personenKonto).Kundennummer = maxKundennummer + neuerKundeLaufnummer;
                        ((Kunde)personenKonto).BestimmeVollerName();
                    }
                }
                else if (type.IsSubclassOf(typeof(Firma)) == true)
                {
                    personenKonto.Name = ZufälligerWertString(Properties.Resources.nachnamen) + " " + ZufälligerWertString(Properties.Resources.firmenRechtsformen);
                    if(personenKonto.GetType() == typeof(Lieferant))
                    {
                        ((Lieferant)personenKonto).Lieferantennummer = maxLieferantennummer + neuerKundeLaufnummer;
                    }
                }

                //-------------------------------------------- child spezifisch --------------------------------------------------
                //-------------------------------------------- parent --------------------------------------------------

                personenKonto.Telefonnummer = ZufälligerWertString(Properties.Resources.Northwind_Phone);
                personenKonto.Email = ZufälligerWertString(Properties.Resources.email);
                personenKonto.Faxnummer = ZufälligerWertString(Properties.Resources.Northwind_Fax);
                personenKonto.IBAN = ZufälligerWertString(Properties.Resources.IBAN);
                string[] dummy = ZufälligerWertundSeineZeilennummerString(Properties.Resources.Postleitzahl);
                int hausnummer = zufallsWertFeld.Next(1, 100);
                personenKonto.Rechnungsadresse_Straße = ZufälligerWertString(Properties.Resources.Straße) + " " + hausnummer;
                personenKonto.Rechnungsadresse_Postleitzahl = dummy[0];
                personenKonto.Rechnungsadresse_Ort = BestimmterWertString(Properties.Resources.Ort, int.Parse(dummy[1]));
                personenKonto.Rechnungsadresse_Landkreis = BestimmterWertString(Properties.Resources.landkreis, int.Parse(dummy[1]));
                personenKonto.Rechnungsadresse_Bundesland = BestimmterWertString(Properties.Resources.bundesland, int.Parse(dummy[1]));
                personenKonto.Rechnungsadresse_Land = "Deutschland";
                personenKonto.LieferungZurRechnungsadresse = true;

                int zufallLieferadresse = zufallsWertFeld.Next(2);
                if (zufallLieferadresse == 0)
                {
                    personenKonto.LieferungZurRechnungsadresse = false;
                    string[] dummyL = ZufälligerWertundSeineZeilennummerString(Properties.Resources.Postleitzahl);
                    int hausnummerL = zufallsWertFeld.Next(1, 100);
                    personenKonto.Lieferadresse_Straße = ZufälligerWertString(Properties.Resources.Straße) + " " + hausnummerL;
                    personenKonto.Lieferadresse_Postleitzahl = dummyL[0];
                    personenKonto.Lieferadresse_Ort = BestimmterWertString(Properties.Resources.Ort, int.Parse(dummyL[1]));
                    personenKonto.Lieferadresse_Landkreis = BestimmterWertString(Properties.Resources.landkreis, int.Parse(dummyL[1]));
                    personenKonto.Lieferadresse_Bundesland = BestimmterWertString(Properties.Resources.bundesland, int.Parse(dummyL[1]));
                    personenKonto.Lieferadresse_Land = "Deutschland";
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auftragserfassung_Blazor.Module.Helpers
{
    class BelegHelper3000
    {
        public BelegHelper3000()
        {

        }

        //public void ÜberprüfeZustandDerBestellung()
        //{
        //    //überprüfen, ob die Bestellung vollständig ausgeliefert wurde

        //    bool unvollständig = false;
        //    foreach (BestellungsPosition bestellungsPosition in BestellungsPositionenListe)
        //    {
        //        if (bestellungsPosition.PositionIstVollständig == false)
        //        {
        //            unvollständig = true;
        //            break;
        //        }
        //    }
        //    if (unvollständig == false)
        //    {
        //        AuftragIstAbgearbeitet = true;
        //        Save();
        //    }
        //    else
        //    {
        //        AuftragIstAbgearbeitet = false;
        //        Save();
        //    }
        //}


        //public void BildeRechnungsSummen() //Summiert die einzelnen Positionen Summen auf und wendet denn den optionalen Summenrabatt an
        //{
        //    SummeRechnungsNetto = 0;
        //    SummeRechnungsBrutto = 0;

        //    foreach (BestellungsPosition posi in BestellungsPositionenListe)
        //    {
        //        //double verwendeterPreis = posi.BestellterArtikelPreis;
        //        //double verwendeterRabatt = posi.Zeilenrabatt;

        //        //if (posi.AktionspreisBestellterArtikel != 0 && BenutzeAktionspreis == true)
        //        //{
        //        //    verwendeterPreis = posi.AktionspreisBestellterArtikel;
        //        //}

        //        //if (BenutzeAktionsrabatt == true)
        //        //{
        //        //    verwendeterRabatt = posi.Zeilenrabatt + posi.AktionsrabattBestellterArtikel;
        //        //}

        //        posi.SummeDerPositionNetto_NP = posi.BestellterArtikelPreis * posi.AnzahlBestellteMenge * (1 + (posi.Zeilenrabatt + Summenrabatt) / 100);
        //        posi.SummeDerPositionBrutto_NP = posi.SummeDerPositionNetto_NP * (1 + posi.BestellterArtikelSteuer / 100);

        //        posi.SummeDerPositionNetto_NP = Math.Round(posi.SummeDerPositionNetto_NP, 4);
        //        posi.SummeDerPositionBrutto_NP = Math.Round(posi.SummeDerPositionBrutto_NP, 4);

        //        SummeRechnungsNetto += posi.SummeDerPositionNetto_NP;
        //        SummeRechnungsBrutto += posi.SummeDerPositionBrutto_NP;
        //    }
        //}
    }
}

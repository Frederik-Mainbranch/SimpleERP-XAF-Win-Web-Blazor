using System;
using DevExpress.ExpressApp.Xpo;

namespace Auftragserfassung_Blazor.Blazor.Server.Services {
    public class XpoDataStoreProviderAccessor {
        public IXpoDataStoreProvider DataStoreProvider { get; set; }
    }
}

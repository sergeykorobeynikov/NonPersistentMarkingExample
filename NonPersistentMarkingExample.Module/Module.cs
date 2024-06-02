using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using NonPersistentMarkingExample.Module.BusinessObjects;
using NonPersistentMarkingExample.Module.Helpers;

namespace NonPersistentMarkingExample.Module;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class NonPersistentMarkingExampleModule : ModuleBase {
    public NonPersistentMarkingExampleModule() {
		// 
		// NonPersistentMarkingExampleModule
		// 
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
        ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
        return new ModuleUpdater[] { updater };
    }
    public override void Setup(XafApplication application) {
        base.Setup(application);
        // Manage various aspects of the application UI and behavior at the module level.
        application.SetupComplete += Application_SetupComplete;
    }

    private void Application_SetupComplete(object sender, EventArgs e)
    {
        Application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
        NonPersistentObjectSpace.UseKeyComparisonToDetermineIdentity = true;
    }

    private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e)
    {
        if (e.ObjectSpace is CompositeObjectSpace cos)
        {
            if (cos.Owner is not CompositeObjectSpace)
            {
                cos.PopulateAdditionalObjectSpaces((XafApplication)sender);
                cos.AutoCommitAdditionalObjectSpaces = true;
                cos.AutoRefreshAdditionalObjectSpaces = true;
            }
        }

        if (e.ObjectSpace is NonPersistentObjectSpace npos)
        {
            npos.AutoSetModifiedOnObjectChange = true;
            _ = new NonPersistentMarkingRowAdapter<Organization>(npos);
            _ = new NonPersistentMarkingRowAdapter<Warehouse>(npos);
            _ = new NonPersistentMarkingRowAdapter<Product>(npos);
        }
    }
}

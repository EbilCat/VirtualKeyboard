using System;
using System.Collections.Generic;
using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;

public class VirtualKeyboardVisibility : BaseSpawnee
{
    private VirtualKeyboardDO virtualKeyboardDO;
    private LevelDataDO levelDataDO;
    private List<ConcurrentTask> mortarConcurrentTasks = new List<ConcurrentTask>();


//*====================
//* BINDING
//*====================
    public override void BindDO(IDataObject dataObject)
    {
        base.BindDO(dataObject);
        this.AttemptDependencyBind(dataObject, ref virtualKeyboardDO);
    }

    public override void UnbindDO(IDataObject dataObject)
    {
        base.UnbindDO(dataObject);
        this.UnbindDependency(dataObject, ref virtualKeyboardDO);
    }

    protected override bool FulfillDependencies()
    {
        bool fulfilled = base.FulfillDependencies();
        fulfilled &= virtualKeyboardDO != null;
        fulfilled &= RetrieveDependency(ref levelDataDO);
        return fulfilled;
    }

    protected override void ClearDependencies(object obj = null)
    {
        base.ClearDependencies(obj);
        this.ClearDependency(ref virtualKeyboardDO);
        this.ClearDependency(ref levelDataDO);
    }

    protected override void RegisterCallbacks()
    {
        DataObjectMasterRepository.RegisterForCreation(OnDataObjectCreated);
        DataObjectMasterRepository.RegisterForDisposing(OnDataObjectDisposing);
        this.levelDataDO.IsSitRepPanelOpen.RegisterForChanges(EvaluateVisibility);
    }

    protected override void UnregisterCallbacks()
    {
        DataObjectMasterRepository.UnregisterFromCreation(OnDataObjectCreated);
        DataObjectMasterRepository.UnregisterFromDisposing(OnDataObjectDisposing);
        this.levelDataDO?.IsSitRepPanelOpen.UnregisterFromChanges(EvaluateVisibility);
    }


//*====================
//* CALLBACKS
//*====================
    private void OnDataObjectCreated(IDataObject obj)
    {
        if(obj is ConcurrentTask concurrentTask && concurrentTask.ConcurrentType.Value == ConcurrentType.Mortar)
        {
            this.mortarConcurrentTasks.Add(concurrentTask);
            concurrentTask.IsSelected.RegisterForChanges(EvaluateVisibility, false);
            this.EvaluateVisibility();
        }
    }

    private void OnDataObjectDisposing(IDataObject obj)
    {
        if(obj is ConcurrentTask concurrentTask && concurrentTask.ConcurrentType.Value == ConcurrentType.Mortar)
        {
            this.mortarConcurrentTasks.Remove(concurrentTask);
            concurrentTask.IsSelected.UnregisterFromChanges(EvaluateVisibility);
            this.EvaluateVisibility();
        }
    }


//*====================
//* CALLBACKS
//*====================
    private void EvaluateVisibility(object obj = null)
    {
        bool isVisible = false;
        
        foreach (var concurrentTask in mortarConcurrentTasks)
        {
            isVisible |= concurrentTask.IsSelected.Value;
        }
        isVisible |= levelDataDO.IsSitRepPanelOpen.Value;

        this.virtualKeyboardDO.IsActive.Value = isVisible;
    }
}

internal class ConcurrentTask
{
    public OBool IsSelected { get; internal set; }
    public OConcurrentType ConcurrentType { get; internal set; }
}

public enum ConcurrentType { Mortar }

    //*====================
    //* ConcurrentType
    //*====================
    [Serializable]
    public class OConcurrentType : ObservableVar<ConcurrentType>
    {
        public OConcurrentType() : base(default(ConcurrentType)) { }
        public OConcurrentType(ConcurrentType initValue) : base(initValue) { }
        public OConcurrentType(ConcurrentType initValue, IDataObject dataObject) : base(initValue, dataObject) { }
        protected override bool AreEqual(ConcurrentType var, ConcurrentType value) { return (var == value); }

        public override void SetValueFromString(string strVal)
        {
        }
    }

internal class LevelDataDO : IDataObject
{
    public OBool IsSitRepPanelOpen { get; internal set; }

    public event Action<IDataObject> disposing;

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
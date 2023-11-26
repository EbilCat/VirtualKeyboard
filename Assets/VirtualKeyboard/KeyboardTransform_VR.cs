using System;
using CoreDev.Framework;
using CoreDev.Observable;
using CoreDev.Sequencing;
using UnityEngine;

public class KeyboardTransform_VR : MonoBehaviour, ISpawnee
{
    [SerializeField] private Vector3 offset_VrHeadset = new Vector3(0.0f, -0.25f, 0.5f);
    private VirtualKeyboardDO virtualKeyboardDO;
    private VRRigDO vrRigDO;
    private TrackingSpaceDO trackingSpaceDO;
    private VRHeadsetDO vrHeadsetDO;

    private Vector3 defaultScale_Local;
    private Vector3 keyboardPos_TrackingSpace;
    private Quaternion keyboardRot_TrackingSpace;



    //*====================
    //* UNITY
    //*====================
    protected virtual void OnDestroy()
    {
        this.UnbindDO(this.virtualKeyboardDO);
    }

    //*====================
    //* BINDING
    //*====================
    public void BindDO(IDataObject dataObject)
    {
        if (dataObject is VirtualKeyboardDO && this.virtualKeyboardDO == null)
        {
            this.virtualKeyboardDO = dataObject as VirtualKeyboardDO;
            this.CompleteBinding();
        }
    }

    private void CompleteBinding(object obj = null)
    {
        this.vrRigDO = DataObjectMasterRepository.GetDataObject<VRRigDO>();
        this.trackingSpaceDO = DataObjectMasterRepository.GetDataObject<TrackingSpaceDO>();
        this.vrHeadsetDO = DataObjectMasterRepository.GetDataObject<VRHeadsetDO>();

        if (this.vrRigDO != null && this.trackingSpaceDO != null && this.vrHeadsetDO != null)
        {
            this.defaultScale_Local = this.transform.localScale;

            this.vrRigDO.Scale_Local.RegisterForChanges(OnScale_LocalChanged);
            this.vrRigDO.Pos_Local.RegisterForChanges(Maintain);
            this.vrRigDO.Rot_Local.RegisterForChanges(Maintain);

            this.trackingSpaceDO.Pos_Local.RegisterForChanges(Maintain);
            this.trackingSpaceDO.Rot_Local.RegisterForChanges(EvaluateKeyboardPosAndRot);

            this.vrHeadsetDO.Pos_Local.RegisterForChanges(EvaluateKeyboardPosAndRot);
            this.vrHeadsetDO.Rot_Local.RegisterForChanges(EvaluateKeyboardPosAndRot);

            UniversalTimer.ScheduleCallback(EvaluateKeyboardPosAndRot, 1.0f);
        }
        else
        {
            UniversalTimer.ScheduleCallback(CompleteBinding, 0.1f);
        }
    }

    public void UnbindDO(IDataObject dataObject)
    {
        if (dataObject is VirtualKeyboardDO && this.virtualKeyboardDO == dataObject as VirtualKeyboardDO)
        {
            this.vrRigDO?.Scale_Local.UnregisterFromChanges(OnScale_LocalChanged);
            this.vrRigDO?.Pos_Local.UnregisterFromChanges(Maintain);
            this.vrRigDO?.Rot_Local.UnregisterFromChanges(Maintain);

            this.trackingSpaceDO?.Pos_Local.UnregisterFromChanges(Maintain);
            this.trackingSpaceDO?.Rot_Local.UnregisterFromChanges(EvaluateKeyboardPosAndRot);

            this.vrHeadsetDO?.Pos_Local.UnregisterFromChanges(EvaluateKeyboardPosAndRot);
            this.vrHeadsetDO?.Rot_Local.UnregisterFromChanges(EvaluateKeyboardPosAndRot);

            this.virtualKeyboardDO = null;
            this.vrRigDO = null;
            this.trackingSpaceDO = null;
            this.vrHeadsetDO = null;
        }
    }


    //*====================
    //* CALLBACKS
    //*====================
    private void OnScale_LocalChanged(ObservableVar<Vector3> obj)
    {
        this.virtualKeyboardDO.Scale_Local.Value = this.defaultScale_Local * obj.Value.x;
    }

    private void Maintain(object obj)
    {
        this.virtualKeyboardDO.Pos_World = this.trackingSpaceDO.ThisToWorld(this.keyboardPos_TrackingSpace);
        this.virtualKeyboardDO.Rot_World = this.trackingSpaceDO.ThisToWorld(this.keyboardRot_TrackingSpace);
    }

    private void EvaluateKeyboardPosAndRot(object obj = null)
    {
        Vector3 flattendHeadsetFwd_World = Vector3.ProjectOnPlane(this.vrHeadsetDO.Forward_World, Vector3.up).normalized;
        Vector3 flattendKeyboardToHeadset_World = Vector3.ProjectOnPlane(this.virtualKeyboardDO.Pos_World - this.vrHeadsetDO.Pos_World, Vector3.up).normalized;

        if (Vector3.Angle(flattendHeadsetFwd_World, flattendKeyboardToHeadset_World) > 15.0f)
        {
            Vector3 flattendHeadsetFwd_VrHeadset = this.vrHeadsetDO.WorldToThisDirection(flattendHeadsetFwd_World);
            Vector3 virtualKeyboardPos_VrHeadset = (flattendHeadsetFwd_VrHeadset * this.offset_VrHeadset.z);
            virtualKeyboardPos_VrHeadset.y += this.offset_VrHeadset.y;

            this.virtualKeyboardDO.Pos_World = this.vrHeadsetDO.ThisToWorld(virtualKeyboardPos_VrHeadset);
            this.virtualKeyboardDO.Rot_World = Quaternion.LookRotation((this.virtualKeyboardDO.Pos_World - this.vrHeadsetDO.Pos_World).normalized, Vector3.up);

            this.keyboardPos_TrackingSpace = this.trackingSpaceDO.WorldToThis(this.virtualKeyboardDO.Pos_World);
            this.keyboardRot_TrackingSpace = this.trackingSpaceDO.WorldToThis(this.virtualKeyboardDO.Rot_World);
        }
    }
}

public class VRHeadsetDO : IDataObject
{
    internal OVector3 Pos_Local;
    internal OQuaternion Rot_Local;

    public Vector3 Pos_World { get; internal set; }
    public Vector3 Forward_World { get; internal set; }

    public event Action<IDataObject> disposing;

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    internal Vector3 ThisToWorld(Vector3 virtualKeyboardPos_VrHeadset)
    {
        throw new NotImplementedException();
    }

    internal Vector3 WorldToThisDirection(Vector3 flattendHeadsetFwd_World)
    {
        throw new NotImplementedException();
    }
}

internal class TrackingSpaceDO : IDataObject
{
    internal OVector3 Pos_Local;
    internal OQuaternion Rot_Local;

    public event Action<IDataObject> disposing;

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    internal Quaternion ThisToWorld(Quaternion keyboardRot_TrackingSpace)
    {
        throw new NotImplementedException();
    }

    internal Vector3 ThisToWorld(Vector3 keyboardPos_TrackingSpace)
    {
        throw new NotImplementedException();
    }

    internal Vector3 WorldToThis(Vector3 pos_World)
    {
        throw new NotImplementedException();
    }

    internal Quaternion WorldToThis(Quaternion rot_World)
    {
        throw new NotImplementedException();
    }
}

internal class VRRigDO : IDataObject
{
    internal OVector3 Scale_Local;
    internal OVector3 Pos_Local;
    internal OQuaternion Rot_Local;

    public event Action<IDataObject> disposing;

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
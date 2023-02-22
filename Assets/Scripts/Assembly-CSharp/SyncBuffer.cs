using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SyncBuffer : MonoBehaviour
{
    public struct Keyframe
    {
        public float InterpolationTime;

        public Vector3 Position;

        public Vector3 Velocity;

        public Vector3 Acceleration;

        public Quaternion Rotation;

        public Vector3 AngularVelocity;

        public Vector3 AngularAcceleration;
    }

    [Range(0.05f, 0.5f)]
    public float TargetLatency = 0.075f;

    [NonSerialized]
    public float ErrorCorrectionSpeed = 10f;

    [NonSerialized]
    public float TimeCorrectionSpeed = 3f;

    [NonSerialized]
    public bool DisableExtrapolation;

    protected float _playbackTime;

    protected float _timeDrift;

    protected List<Keyframe> _keyframes = new List<Keyframe>();

    [NonSerialized]
    public Vector3 ExtrapolationPositionDrift;

    [NonSerialized]
    public Quaternion ExtrapolationRotationDrift;

    public virtual bool IsExtrapolating
    {
        get
        {
            return _keyframes.Count == 1;
        }
    }

    public virtual Vector3 Position
    {
        get
        {
            return PositionNoErrorCorrection + ExtrapolationPositionDrift;
        }
    }

    public virtual Vector3 PositionNoErrorCorrection
    {
        get
        {
            if (!HasKeyframes)
            {
                Debug.LogWarning("Trying to access position in an empty buffer. Zero vector returned.");
                return Vector3.zero;
            }
            if (DisableExtrapolation && IsExtrapolating)
            {
                return _keyframes[0].Position;
            }
            return _keyframes[0].Position + _keyframes[0].Velocity * _playbackTime + 0.5f * _keyframes[0].Acceleration * _playbackTime * _playbackTime;
        }
    }

    public virtual Vector3 Velocity
    {
        get
        {
            if (!HasKeyframes)
            {
                Debug.LogWarning("Trying to access velocity in an empty buffer. Zero vector returned.");
                return Vector3.zero;
            }
            return _keyframes[0].Velocity + _keyframes[0].Acceleration * _playbackTime;
        }
    }

    public virtual Quaternion Rotation
    {
        get
        {
            return RotationNoErrorCorrection * ExtrapolationRotationDrift;
        }
    }

    public virtual Quaternion RotationNoErrorCorrection
    {
        get
        {
            if (!HasKeyframes)
            {
                Debug.LogWarning("Trying to access rotation in an empty buffer. Zero rotation returned.");
                return Quaternion.identity;
            }
            if (DisableExtrapolation && IsExtrapolating)
            {
                return _keyframes[0].Rotation;
            }
            return _keyframes[0].Rotation * Quaternion.Euler(_keyframes[0].AngularVelocity * _playbackTime + 0.5f * _keyframes[0].AngularAcceleration * _playbackTime * _playbackTime);
        }
    }

    public virtual Vector3 AngularVelocity
    {
        get
        {
            if (!HasKeyframes)
            {
                Debug.LogWarning("Trying to access angular velocity in an empty buffer. Zero vector returned.");
                return Vector3.zero;
            }
            return _keyframes[0].AngularVelocity + _keyframes[0].AngularAcceleration * _playbackTime;
        }
    }

    public virtual bool HasKeyframes
    {
        get
        {
            return _keyframes.Count != 0;
        }
    }

    public virtual Keyframe LastReceivedKeyframe
    {
        get
        {
            if (!HasKeyframes)
            {
                Debug.LogWarning("Trying to access LastReceivedKeyframe in an empty buffer. Blank keyframe returned.");
                return default(Keyframe);
            }
            return _keyframes[_keyframes.Count - 1];
        }
    }

    public virtual Keyframe NextKeyframe
    {
        get
        {
            if (_keyframes.Count < 2)
            {
                Debug.LogWarning("Trying to access NextKeyframe in a buffer that is empty or currently extrapolating. Blank keyframe returned.");
                return default(Keyframe);
            }
            return _keyframes[1];
        }
    }

    public virtual Keyframe CurrentKeyframe
    {
        get
        {
            if (!HasKeyframes)
            {
                Debug.LogWarning("Trying to access CurrentKeyframe in an empty buffer. Blank keyframe returned.");
                return default(Keyframe);
            }
            return _keyframes[0];
        }
    }

    public virtual void AddKeyframe(float interpolationTime, Vector3 position, [Optional] Quaternion rotation, [Optional] Vector3 acceleration, [Optional] Vector3 angularAcceleration, Vector3? velocity = null, Vector3? angularVelocity = null)
    {
        if (_keyframes.Count < 1)
        {
            interpolationTime = Mathf.Max(TargetLatency, 0.01f);
        }
        float num = interpolationTime - _playbackTime;
        for (int i = 1; i < _keyframes.Count; i++)
        {
            num += _keyframes[i].InterpolationTime;
        }
        _timeDrift = TargetLatency - num;
        Keyframe keyframe;
        if (_keyframes.Count < 1)
        {
            keyframe = default(Keyframe);
            keyframe.Position = position;
            keyframe.Velocity = Vector3.zero;
            keyframe.Acceleration = Vector3.zero;
            keyframe.Rotation = rotation;
            keyframe.AngularVelocity = Vector3.zero;
            keyframe.AngularAcceleration = Vector3.zero;
            Keyframe item = keyframe;
            _keyframes.Add(item);
        }
        Vector3 position2 = Position;
        Quaternion rotation2 = Rotation;
        Keyframe lastReceivedKeyframe = LastReceivedKeyframe;
        Vector3 vector = ((!(interpolationTime > 0f)) ? Vector3.zero : ((position - lastReceivedKeyframe.Position) / interpolationTime));
        Quaternion rotationDifference = GetRotationDifference(lastReceivedKeyframe.Rotation, rotation);
        Vector3 vector2 = ((!(interpolationTime > 0f)) ? Vector3.zero : (FormatEulerRotation180(rotationDifference.eulerAngles) / interpolationTime));
        keyframe = default(Keyframe);
        keyframe.InterpolationTime = interpolationTime;
        keyframe.Position = position;
        keyframe.Velocity = ((!velocity.HasValue) ? vector : velocity.Value);
        keyframe.Acceleration = acceleration;
        keyframe.Rotation = rotation;
        keyframe.AngularVelocity = ((!angularVelocity.HasValue) ? vector2 : angularVelocity.Value);
        keyframe.AngularAcceleration = angularAcceleration;
        Keyframe item2 = keyframe;
        _keyframes.Add(item2);
        lastReceivedKeyframe.Velocity = vector;
        lastReceivedKeyframe.AngularVelocity = vector2;
        lastReceivedKeyframe.Acceleration = Vector3.zero;
        lastReceivedKeyframe.AngularAcceleration = Vector3.zero;
        _keyframes[_keyframes.Count - 2] = lastReceivedKeyframe;
        UpdatePlayback(0f);
        Vector3 positionNoErrorCorrection = PositionNoErrorCorrection;
        Quaternion rotationNoErrorCorrection = RotationNoErrorCorrection;
        ExtrapolationPositionDrift = position2 - positionNoErrorCorrection;
        ExtrapolationRotationDrift = GetRotationDifference(rotationNoErrorCorrection, rotation2);
    }

    public virtual void UpdatePlayback(float deltaTime)
    {
        if (_keyframes.Count < 1)
        {
            Debug.LogWarning("Trying to update playback in an empty buffer.");
            return;
        }
        if (deltaTime > 0f)
        {
            _playbackTime += deltaTime;
            float num = 0f - Mathf.Lerp(0f, _timeDrift, TimeCorrectionSpeed * deltaTime);
            _playbackTime += num;
            _timeDrift += num;
            ExtrapolationPositionDrift = Vector3.Lerp(ExtrapolationPositionDrift, Vector3.zero, ErrorCorrectionSpeed * deltaTime);
            ExtrapolationRotationDrift = Quaternion.Lerp(ExtrapolationRotationDrift, Quaternion.identity, ErrorCorrectionSpeed * deltaTime);
        }
        while (_keyframes.Count > 1 && _playbackTime >= _keyframes[1].InterpolationTime)
        {
            if (_keyframes[1].InterpolationTime == 0f)
            {
                ExtrapolationPositionDrift = Vector3.zero;
                ExtrapolationRotationDrift = Quaternion.identity;
            }
            _playbackTime -= _keyframes[1].InterpolationTime;
            _keyframes.RemoveAt(0);
        }
    }

    protected Quaternion GetRotationDifference(Quaternion fromRotation, Quaternion toRotation)
    {
        return Quaternion.Inverse(fromRotation) * toRotation;
    }

    protected Vector3 FormatEulerRotation180(Vector3 eulerRotation)
    {
        while (eulerRotation.x > 180f)
        {
            eulerRotation.x -= 360f;
        }
        while (eulerRotation.y > 180f)
        {
            eulerRotation.y -= 360f;
        }
        while (eulerRotation.z > 180f)
        {
            eulerRotation.z -= 360f;
        }
        while (eulerRotation.x <= -180f)
        {
            eulerRotation.x += 360f;
        }
        while (eulerRotation.y <= -180f)
        {
            eulerRotation.y += 360f;
        }
        while (eulerRotation.z <= -180f)
        {
            eulerRotation.z += 360f;
        }
        return eulerRotation;
    }
}

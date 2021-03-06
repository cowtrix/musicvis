﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRenderer3D : LineRenderer3D
{
    public int PointLimit = 100;
    public float DistanceLimit = 100;
    public float UpdateThreshold = 1;

    private Vector3 _lastNewPointPosition;

    protected override void Awake()
    {
        base.Awake();
        OnPositionChanged.AddListener(OnPositionChangedCallbck);
    }

    private void OnPositionChangedCallbck()
    {
        RebakeMesh();
    }

    protected override int GetMaxPoints()
    {
        return PointLimit;
    }

    protected override void Update()
    {
        if ((_lastNewPointPosition - CurrentTransform.position).magnitude > UpdateThreshold)
        {
            _lastNewPointPosition = CurrentTransform.position;
            Points.Add(GetPointState());
        }
        else if (Points.Count > 1)
        {
            Points[Points.Count - 1] = GetPointState();
        }
        while (Points.Count > PointLimit)
        {
            Points.RemoveAt(0);
        }

        float distanceAccum = 0;
        for(var i = Points.Count - 2; i >= 0; i--)
        {
            if(distanceAccum > DistanceLimit)
            {
                Points.RemoveAt(i);
            }
            distanceAccum += Vector3.Distance(Points[i].Position, Points[i+1].Position);
        }

        base.Update();
    }
}

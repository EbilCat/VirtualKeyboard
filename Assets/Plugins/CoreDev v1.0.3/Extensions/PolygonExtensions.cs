﻿using UnityEngine;



namespace CoreDev.Extensions
{
    public static class PolygonExtensions
    {
        public static bool ContainsPoint(this Vector2[] polyPoints, Vector2 p)
        {
            var j = polyPoints.Length - 1;
            var inside = false;
            for (int i = 0; i < polyPoints.Length; j = i++)
            {
                var pi = polyPoints[i];
                var pj = polyPoints[j];
                
                if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) && (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                {
                    inside = !inside;
                }
            }
            return inside;
        }
    }
}
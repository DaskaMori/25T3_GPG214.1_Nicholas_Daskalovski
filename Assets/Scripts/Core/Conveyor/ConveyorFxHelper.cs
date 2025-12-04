using UnityEngine;

namespace Core.Conveyor
{
    public static class ConveyorFxHelper
    {
        public static ParticleSystem SpawnFx(ParticleSystem prefab, Transform anchor, Color tint, float seconds)
        {
            if (!prefab) return null;

            var t = anchor ? anchor : prefab.transform; 
            var fx = Object.Instantiate(prefab, t.position, t.rotation);
            if (anchor) fx.transform.SetParent(anchor, worldPositionStays: true);
            
            var main = fx.main;
            main.startColor = tint;

            fx.Play();
            Object.Destroy(fx.gameObject, seconds + 0.25f); 
            return fx;
        }
    }
    
}
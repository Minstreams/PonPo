using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("little Components/ParticleEmitter")]
public class ParticleEmitter : MonoBehaviour
{
    [System.Serializable]
    public class ParticlePair
    {
        public ParticleSystem particle;
        public int count = 1;
    }

    public List<ParticlePair> particles = new List<ParticlePair>();


    public void Emit()
    {
        foreach (ParticlePair pp in particles)
        {
            pp.particle.Emit(pp.count);
        }
    }
}

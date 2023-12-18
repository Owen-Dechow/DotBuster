using System.Collections.Generic;
using UnityEngine;


public class ParticlesPool : MonoBehaviour
{
    [SerializeField] int numberOfUnits;
    [SerializeField] GameObject particleUnit;

    private List<ParticlesUnit> particles;
    private static ParticlesPool _i;

    private void Start()
    {
        _i = this;
        particles = new List<ParticlesUnit>();

        numberOfUnits = Mathf.Clamp(numberOfUnits, 1, int.MaxValue);
        for (int i = 0; i < numberOfUnits; i++)
        {
            GameObject go = Instantiate(particleUnit, transform);
            particles.Add(new ParticlesUnit(go));
        }
    }

    struct ParticlesUnit
    {
        public Transform t;
        public ParticleSystem ps;
        public AudioSource aus;
        public GameObject go;

        public ParticlesUnit(GameObject go)
        {
            t = go.transform;
            ps = go.GetComponent<ParticleSystem>();
            aus = go.GetComponent<AudioSource>();
            this.go = go;
        }
    }

    public static void PlayAtPoint(Vector3 position, Color color)
    {
        _i.PrivatePlayAtPoint(position, color);
    }

    void PrivatePlayAtPoint(Vector3 position, Color color)
    {
        ParticlesUnit particlesUnit = particles[0];
        particles.RemoveAt(0);
        particles.Add(particlesUnit);

        particlesUnit.ps.Stop();
        particlesUnit.t.position = position;

        ParticleSystem.MainModule main = particlesUnit.ps.main;
        main.startColor = color;

        particlesUnit.ps.Play();

        if (GameManager.SoundOn)
        {
            particlesUnit.aus.volume = GameManager.SFXVol;
            particlesUnit.aus.Play();
        }
    }
}

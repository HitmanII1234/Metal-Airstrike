using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PowerUpType
{
    // Temporales
    TripleShot, MegaLaser, HomingMissiles, ChainLightning, BombBullets,
    // Permanentes
    HealthBoost, AutoRegen, EnergyShield, LifeSteal, BulletImmunity, Magnetism, OrbitalDrone, MineTrail
}

[CreateAssetMenu(fileName = "NewPowerUp", menuName = "PowerUp")]
public class PowerUpData : ScriptableObject
{
    public PowerUpType tipo;
    public string nombre;
    public string descripcion;
    public Sprite icono;
    public float duracion; // 0 = permanente
}

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    public List<PowerUpData> todosLosPoderes;
    
    // Estados activos
    public bool tripleShotActivo = false;
    public bool megaLaserActivo = false;
    public bool homingMissilesActivo = false;
    public bool chainLightningActivo = false;
    public bool bombBulletsActivo = false;
    
    public bool mineTrailActivo = false;

    private Salud jugadorSalud;
    private Coroutine autoRegenCoroutine;
    private Coroutine mineTrailCoroutine;
    private Dictionary<PowerUpType, float> temporizadoresActivos = new Dictionary<PowerUpType, float>();

    void Awake()
    {
        Instance = this;
        ResetPoderes();
    }

    void Start()
    {
        GameObject j = GameObject.FindGameObjectWithTag("Player");
        if (j != null) jugadorSalud = j.GetComponent<Salud>();
    }

    void Update()
    {
        if (temporizadoresActivos.Count == 0)
            return;

        List<PowerUpType> tipos = new List<PowerUpType>(temporizadoresActivos.Keys);
        foreach (PowerUpType tipo in tipos)
        {
            temporizadoresActivos[tipo] -= Time.deltaTime;
            if (temporizadoresActivos[tipo] <= 0f)
            {
                temporizadoresActivos.Remove(tipo);
                ActivarTemporal(tipo, false);
            }
        }
    }

    public void ApplyPowerUp(PowerUpData data)
    {
        if (data.duracion > 0)
        {
            if (temporizadoresActivos.ContainsKey(data.tipo))
            {
                temporizadoresActivos[data.tipo] += data.duracion;
            }
            else
            {
                temporizadoresActivos[data.tipo] = data.duracion;
                ActivarTemporal(data.tipo, true);
            }
        }
        else
        {
            ApplyPermanent(data.tipo);
        }
    }

    void ActivarTemporal(PowerUpType tipo, bool estado)
    {
        switch (tipo)
        {
            case PowerUpType.TripleShot: tripleShotActivo = estado; break;
            case PowerUpType.MegaLaser: megaLaserActivo = estado; break;
            case PowerUpType.HomingMissiles: homingMissilesActivo = estado; break;
            case PowerUpType.ChainLightning: chainLightningActivo = estado; break;
            case PowerUpType.BombBullets: bombBulletsActivo = estado; break;
        }
    }

    void ApplyPermanent(PowerUpType tipo)
    {
        switch (tipo)
        {
            case PowerUpType.HealthBoost:
                if (jugadorSalud != null) jugadorSalud.AumentarVidaMaxima(0.05f);
                break;
            case PowerUpType.AutoRegen:
                if (autoRegenCoroutine == null) autoRegenCoroutine = StartCoroutine(AutoRegenRoutine());
                break;
            case PowerUpType.LifeSteal:
                GameManager.Instance.hasLifeSteal = true;
                break;
            case PowerUpType.BulletImmunity:
                GameManager.Instance.bulletImmunityChance += 0.1f; // 10% probabilidad
                break;
            case PowerUpType.Magnetism:
                GameManager.Instance.hasMagnetism = true;
                break;
            case PowerUpType.MineTrail:
                mineTrailActivo = true;
                if (mineTrailCoroutine == null) mineTrailCoroutine = StartCoroutine(MineTrailRoutine());
                break;
            // Otros (EnergyShield, OrbitalDrone) requerirían sus propios GameObjects/Logicas
        }
    }

    IEnumerator AutoRegenRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (jugadorSalud != null) jugadorSalud.Curar(10f);
        }
    }

    IEnumerator MineTrailRoutine()
    {
        while (mineTrailActivo)
        {
            yield return new WaitForSeconds(2f);
            GameObject j = GameObject.FindGameObjectWithTag("Player");
            if (j != null && ObjectPool.Instance != null)
            {
                ObjectPool.Instance.SpawnFromPool("Mina", j.transform.position, Quaternion.identity);
            }
        }
    }

    public void ResetPoderes()
    {
        temporizadoresActivos.Clear();
        tripleShotActivo = false;
        megaLaserActivo = false;
        homingMissilesActivo = false;
        chainLightningActivo = false;
        bombBulletsActivo = false;
        mineTrailActivo = false;

        if (autoRegenCoroutine != null)
        {
            StopCoroutine(autoRegenCoroutine);
            autoRegenCoroutine = null;
        }

        if (mineTrailCoroutine != null)
        {
            StopCoroutine(mineTrailCoroutine);
            mineTrailCoroutine = null;
        }
    }
}

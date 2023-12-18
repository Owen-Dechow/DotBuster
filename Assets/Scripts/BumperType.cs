using UnityEngine;

[System.Serializable]
public class BumperType
{
    public enum Action
    {
        Normal,
        KillVelocity,
        SlowDownTime,
        SpeedUpTime,
        ShrinkPlayer,
        EnlargePlayer,
        Magnet,
        KillPlayer,
    }

    public Action action;
    public int points;
    public Color color;
    public Sprite sprite;
    public float scaleAdjuster;
    public bool magnetic;

    public void Execute(Vector2 position)
    {
        ParticlesPool.PlayAtPoint(position, color);
        UIController.AddToScore(points);
        bool killVelocity = action == Action.KillVelocity;
        Player.ChangeVelocity(killVelocity);

        if (GameManager.PowerTypeEnabled(action))
            return;

        switch (action)
        {
            case Action.SlowDownTime:
                Time.timeScale -= .7f;
                GameManager.LoadPower(action, () => { Time.timeScale += .7f; });
                break;
            case Action.SpeedUpTime:
                Time.timeScale += .7f;
                GameManager.LoadPower(action, () => { Time.timeScale -= .7f; });
                break;
            case Action.ShrinkPlayer:
                Player.BaseScale -= .5f;
                GameManager.LoadPower(action, () => { Player.BaseScale += .5f; });
                break;
            case Action.EnlargePlayer:
                Player.BaseScale += 2;
                GameManager.LoadPower(action, () => { Player.BaseScale -= 2; });
                break;
            case Action.Magnet:
                Player.Magnet = true;
                GameManager.LoadPower(action, () => { Player.Magnet = false; });
                break;
            case Action.KillPlayer:
                Player.KillPlayer();
                break;
        }

    }
}

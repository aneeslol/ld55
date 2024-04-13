using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] BallController Ball;
        [SerializeField] TMP_Text ScoreText;
        [SerializeField] ManaBarController ManaBar;
        [SerializeField] TMP_Text CountdownText;

        [SerializeField] GameObject PaddlePrefab;
        [SerializeField] GameObject RunnerPrefab;
        [SerializeField] GameObject SummoningCirclePrefab;
        [SerializeField] GameObject ScoreEffectPrefab;

        [SerializeField] float Mana;
        [SerializeField] float MaxMana;

        [SerializeField] float EnemyMana;
        [SerializeField] float EnemyMaxMana;

        int Player0Score;
        int Player1Score;

        const int PADDLE_COST = 80;
        const int RUNNER_COST = 40;

        List<BaseMinionController> MinionControllers = new();
        System.Random RNG = new System.Random();

        private void OnDestroy()
        {
            BallController.ScoredGoal -= BallController_ScoredGoal;
            FloorController.FloorClicked -= FloorController_FloorClicked;
            BaseMinionController.OnDeath -= BaseMinionController_OnDeath;
        }

        private void Awake()
        {
            BallController.ScoredGoal += BallController_ScoredGoal;
            FloorController.FloorClicked += FloorController_FloorClicked;
            BaseMinionController.OnDeath += BaseMinionController_OnDeath;
        }

        private void BaseMinionController_OnDeath(BaseMinionController controller)
        {
            MinionControllers.Remove(controller);
        }

        private void FloorController_FloorClicked(int button, Vector3 location)
        {
            if (button == 0)
            {
                SpawnMinion(RUNNER_COST, location, 0, "Runner");
            }
            else if (button == 1)
            {
                SpawnMinion(PADDLE_COST, location, 0, "Paddle");
            }
        }

        private void SpawnMinion(int cost, Vector3 location, int player, string type)
        {
            if ((player == 0 && Mana < cost) || (player == 1 && EnemyMana < cost))
            {
                return;
            }

            if (player == 0)
                AddMana(-cost);
            else
                AddEnemyMana(-cost);

            location.y = .75f;
            Instantiate(SummoningCirclePrefab, location, Quaternion.identity);

            DOTween.Sequence()
                .OnComplete(() =>
                {
                    if (type == "Paddle")
                    {
                        var paddle = Instantiate(PaddlePrefab, location, Quaternion.identity);
                        var controller = paddle.GetComponent<KeeperController>();
                        MinionControllers.Add(controller);
                        controller.SetPlayer(player);
                    }
                    else if (type == "Runner")
                    {
                        var runner = Instantiate(RunnerPrefab, location, Quaternion.identity);
                        var controller = runner.GetComponent<RunnerController>();
                        MinionControllers.Add(controller);
                        controller.SetPlayer(player);
                    }
                })
                .SetDelay(1);
        }

        private void Start()
        {
            CountdownServe(RNG.Next(0, 2));
        }

        private void Update()
        {
            AddMana(.1f);
            AddEnemyMana(.1f);

            var summon = RNG.Next(0, 1000);
            var xMin = 2.75f;
            var xMax = 11.75f;
            var zMin = -8f;
            var zMax = 7.75f;
            var summonLocation = new Vector3(Random.Range(xMin, xMax), .75f, Random.Range(zMin, zMax));
            if (summon < 3)
            {
                if (MinionControllers.Count(c => c.Player == 1 && c is KeeperController) == 5)
                {
                    SpawnMinion(RUNNER_COST, summonLocation, 1, "Runner");
                    return;
                }
                do
                {
                    summonLocation.x = (RNG.Next(0, 5) * 2) + 2.75f;
                }
                while (MinionControllers.Any(c => 
                    c.Player == 1 
                    && c is KeeperController 
                    && c.transform.position.x == summonLocation.x));
                SpawnMinion(PADDLE_COST, summonLocation, 1, "Paddle");
            }
            else if (summon == 4 && MinionControllers.Count(c => c.Player == 0 && c is KeeperController) > 0)
            {
                SpawnMinion(RUNNER_COST, summonLocation, 1, "Runner");
            }
        }

        private void AddMana(float amount)
        {
            Mana += amount;
            if (Mana < 0) Mana = 0;
            if (Mana > MaxMana) Mana = MaxMana;
            UpdateManaBar();
        }

        private void AddEnemyMana(float amount)
        {
            EnemyMana += amount;
            if (EnemyMana < 0) EnemyMana = 0;
            if (EnemyMana > EnemyMaxMana) EnemyMana = EnemyMaxMana;
            UpdateManaBar();
        }

        private void UpdateManaBar()
        {
            ManaBar.UpdateManaBar($"{(int)Mana} / {(int)MaxMana}", (Mana / MaxMana));
        }

        private void BallController_ScoredGoal(int scoringPlayer)
        {
            if (scoringPlayer == 0)
                Player0Score++;
            else
                Player1Score++;
            UpdateScore();
            Instantiate(ScoreEffectPrefab, Ball.gameObject.transform.position, Quaternion.identity);

            Ball.gameObject.SetActive(false);
            CountdownServe(scoringPlayer);
        }

        private void CountdownServe(int scoringPlayer)
        {
            CountdownText.text = "3";
            CountdownText.fontSize = 144;
            CountdownText.gameObject.SetActive(true);
            DOTween.To(() => CountdownText.fontSize, v => CountdownText.fontSize = v, 0, 1)
                .OnComplete(() =>
                {
                    CountdownText.text = "2";
                    CountdownText.fontSize = 144;
                });

            DOTween.To(() => CountdownText.fontSize, v => CountdownText.fontSize = v, 0, 1)
                .OnComplete(() =>
                {
                    CountdownText.text = "1";
                    CountdownText.fontSize = 144;
                })
                .SetDelay(1);

            DOTween.To(() => CountdownText.fontSize, v => CountdownText.fontSize = v, 0, 1)
                .OnComplete(() =>
                {
                    CountdownText.text = "GO!";
                    CountdownText.fontSize = 144;
                    ServeBall(scoringPlayer);
                })
                .SetDelay(2);

            DOTween.To(() => CountdownText.fontSize, v => CountdownText.fontSize = v, 0, .5f)
                .OnComplete(() => CountdownText.gameObject.SetActive(false))
                .SetDelay(3);
        }

        private void ServeBall(int scoringPlayer)
        {
            Ball.gameObject.SetActive(true);
            Ball.gameObject.transform.position = new Vector3(0, .5f, 0);
            Ball.ResetSpeed();

            var xSpeed = scoringPlayer == 0 ? -1 : 1;
            var zSpeed = Random.Range(0, 2) == 0 ? -1 : 1;
            Ball.SetDirection(new Vector3(xSpeed, 0, zSpeed));
        }

        private void UpdateScore()
        {
            ScoreText.text = $"{Player0Score} - {Player1Score}";
        }
    }
}

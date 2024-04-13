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
        [SerializeField] TMP_Text ManaText;
        [SerializeField] TMP_Text EnemyManaText;

        [SerializeField] GameObject PaddlePrefab;
        [SerializeField] GameObject RunnerPrefab;

        [SerializeField] float Mana;
        [SerializeField] float MaxMana;

        [SerializeField] float EnemyMana;
        [SerializeField] float EnemyMaxMana;

        int Player0Score;
        int Player1Score;

        const int PADDLE_COST = 80;
        const int RUNNER_COST = 40;

        List<BaseMinionController> MinionControllers = new();

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

            if (type == "Paddle")
            {
                var paddle = Instantiate(PaddlePrefab, location, Quaternion.identity);
                var controller = paddle.GetComponent<PaddleController>();
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
        }

        private void Start()
        {
            ServeBall(Random.Range(0, 2));
        }

        private void Update()
        {
            AddMana(.1f);
            AddEnemyMana(.1f);

            var summon = Random.Range(0, 1000);
            var xMin = 2.75f;
            var xMax = 11.75f;
            var zMin = -8f;
            var zMax = 7.75f;
            var summonLocation = new Vector3(Random.Range(xMin, xMax), .75f, Random.Range(zMin, zMax));
            if (summon < 3)
            {
                if (MinionControllers.Count(c => c.Player == 1 && c is PaddleController) == 5)
                {
                    SpawnMinion(RUNNER_COST, summonLocation, 1, "Runner");
                    return;
                }
                summonLocation.x = (int)summonLocation.x;
                while (MinionControllers.Any(c => c.Player == 1 && c is PaddleController && (int)c.transform.position.x == summonLocation.x))
                {
                    summonLocation.x = (int)Random.Range(xMin, xMax);
                }
                SpawnMinion(PADDLE_COST, summonLocation, 1, "Paddle");
            }
            else if (summon == 4 && MinionControllers.Count(c => c.Player == 0 && c is PaddleController) > 0)
            {
                SpawnMinion(RUNNER_COST, summonLocation, 1, "Runner");
            }
        }

        private void AddMana(float amount)
        {
            Mana += amount;
            if (Mana < 0) Mana = 0;
            if (Mana > MaxMana) Mana = MaxMana;
            UpdateManaText();
        }

        private void AddEnemyMana(float amount)
        {
            EnemyMana += amount;
            if (EnemyMana < 0) EnemyMana = 0;
            if (EnemyMana > EnemyMaxMana) EnemyMana = EnemyMaxMana;
            UpdateManaText();
        }

        private void UpdateManaText()
        {
            ManaText.text = $"{(int)Mana} / {(int)MaxMana}";
            EnemyManaText.text = $"{(int)EnemyMana} / {(int)EnemyMaxMana}";
        }

        private void BallController_ScoredGoal(int scoringPlayer)
        {
            if (scoringPlayer == 0)
                Player0Score++;
            else
                Player1Score++;

            UpdateScore();
            ServeBall(scoringPlayer);
        }

        private void ServeBall(int scoringPlayer)
        {
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

using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
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
        [SerializeField] GameObject FetcherPrefab;
        [SerializeField] GameObject PikePrefab;
        [SerializeField] GameObject SummoningCirclePrefab;
        [SerializeField] GameObject ScoreEffectPrefab;

        [SerializeField] float Mana;
        [SerializeField] float MaxMana;

        [SerializeField] float EnemyMana;
        [SerializeField] float EnemyMaxMana;

        [SerializeField] SummonIconController RunnerIcon;
        [SerializeField] SummonIconController KeeperIcon;
        [SerializeField] SummonIconController FetcherIcon;
        [SerializeField] SummonIconController PikeIcon;

        [SerializeField] AudioSource CountdownAudio;
        [SerializeField] AudioSource CountdownGoAudio;
        [SerializeField] AudioSource GoalAudio;

        [SerializeField] public AudioMixer AudioMixer;
        [SerializeField] public TMP_Text VolumeText;

        int Player0Score;
        int Player1Score;
        bool GameOver = false;

        const int KEEPER_COST = 80;
        const int RUNNER_COST = 40;
        const int FETCHER_COST = 60;
        const int PIKE_COST = 50;
        const int MAX_SCORE = 11;

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
            if (button == 1)
            {
                SpawnMinion(RUNNER_COST, location, 0, "Runner");
            }
            else if (button == 2)
            {
                SpawnMinion(KEEPER_COST, location, 0, "Keeper");
            }
            else if (button == 3)
            {
                SpawnMinion(FETCHER_COST, location, 0, "Fetcher");
            }
            else if (button == 4)
            {
                SpawnMinion(PIKE_COST, location, 0, "Pike");
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
                    if (type == "Keeper")
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
                    else if (type == "Fetcher")
                    {
                        var runner = Instantiate(FetcherPrefab, location, Quaternion.identity);
                        var controller = runner.GetComponent<FetcherController>();
                        controller.Ball = Ball;
                        MinionControllers.Add(controller);
                        controller.SetPlayer(player);
                    }
                    else if (type == "Pike")
                    {
                        var runner = Instantiate(PikePrefab, location, Quaternion.identity);
                        var controller = runner.GetComponent<PikeController>();
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
            if (GameOver)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Time.timeScale = 1;
                    SceneManager.LoadScene(0);
                }
                return;
            }

            AddMana(.075f);
            AddEnemyMana(.075f);

            var summon = RNG.Next(0, 1000);
            var xMin = 2.75f;
            var xMax = 11.75f;
            var zMin = -8f;
            var zMax = 7.75f;
            var summonLocation = new Vector3(Random.Range(xMin, xMax), .75f, Random.Range(zMin, zMax));
            var spawnNonKeeper = false;
            var keeperCount = MinionControllers.Count(c => c.Player == 1 && c is KeeperController);
            if (summon < 3)
            {
                if (keeperCount == 5 || (keeperCount == 3 && summon == 2))
                {
                    spawnNonKeeper = true;
                }
                else
                {
                    do
                    {
                        summonLocation.x = (RNG.Next(0, 5) * 2) + 2.75f;
                    }
                    while (MinionControllers.Any(c =>
                        c.Player == 1
                        && c is KeeperController
                        && c.transform.position.x == summonLocation.x));
                    SpawnMinion(KEEPER_COST, summonLocation, 1, "Keeper");
                }
            }

            if (summon == 3 || spawnNonKeeper)
            {
                var pool = new List<string> { "Fetcher" };
                if (MinionControllers.Count(c => c.Player == 0 && c is KeeperController) > 0)
                {
                    pool.Add("Runner");
                    pool.Add("Runner");
                    pool.Add("Runner");
                }

                if (MinionControllers.Count(c => c.Player == 1 && c is KeeperController) > 0)
                {
                    pool.Add("Pike");
                }
                var unit = pool[RNG.Next(0, pool.Count)];

                if (unit == "Runner")
                {
                    SpawnMinion(RUNNER_COST, summonLocation, 1, "Runner");
                }
                else if(unit == "Fetcher")
                {
                    SpawnMinion(FETCHER_COST, summonLocation, 1, "Fetcher");
                }
                else if(unit == "Pike")
                {
                    summonLocation = new Vector3(Random.Range(xMin, xMin + 1), .75f, Random.Range(zMin, zMax));
                    SpawnMinion(PIKE_COST, summonLocation, 1, "Pike");
                }
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
            RunnerIcon.SetActive(Mana > RUNNER_COST);
            KeeperIcon.SetActive(Mana > KEEPER_COST);
            FetcherIcon.SetActive(Mana > FETCHER_COST);
            PikeIcon.SetActive(Mana > PIKE_COST);
        }

        private void BallController_ScoredGoal(int scoringPlayer)
        {
            if (scoringPlayer == 0)
                Player0Score++;
            else
                Player1Score++;
            UpdateScore();
            GoalAudio.Play();

            if (Player0Score == MAX_SCORE)
            {
                DOTween.KillAll();
                CountdownText.text = "Red Wins!";
                CountdownText.fontSize = 144;
                CountdownText.gameObject.SetActive(true);
                Time.timeScale = 0;
                GameOver = true;
                return;
            }
            else if (Player1Score == MAX_SCORE)
            {
                DOTween.KillAll();
                CountdownText.text = "Blue Wins!";
                CountdownText.fontSize = 144;
                CountdownText.gameObject.SetActive(true);
                Time.timeScale = 0;
                GameOver = true;
                return;
            }

            Instantiate(ScoreEffectPrefab, Ball.gameObject.transform.position, Quaternion.identity);

            Ball.gameObject.SetActive(false);
            DOTween.Sequence()
                .OnComplete(() =>
                {
                    CountdownServe(scoringPlayer);
                })
                .SetDelay(2);
        }

        private void CountdownServe(int scoringPlayer)
        {
            CountdownText.text = "3";
            CountdownText.fontSize = 144;
            CountdownText.gameObject.SetActive(true);
            CountdownAudio.Play();
            DOTween.To(() => CountdownText.fontSize, v => CountdownText.fontSize = v, 0, 1)
                .OnComplete(() =>
                {
                    CountdownText.text = "2";
                    CountdownText.fontSize = 144;
                    CountdownAudio.Play();
                });

            DOTween.To(() => CountdownText.fontSize, v => CountdownText.fontSize = v, 0, 1)
                .OnComplete(() =>
                {
                    CountdownText.text = "1";
                    CountdownText.fontSize = 144;
                    CountdownAudio.Play();
                })
                .SetDelay(1);

            DOTween.To(() => CountdownText.fontSize, v => CountdownText.fontSize = v, 0, 1)
                .OnComplete(() =>
                {
                    CountdownText.text = "GO!";
                    CountdownText.fontSize = 144;
                    ServeBall(scoringPlayer);
                    CountdownGoAudio.Play();
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

        public void Sounds_Clicked()
        {
            var floatName = "Volume";
            AudioMixer.GetFloat(floatName, out var volume);
            if (volume == 0)
            {
                AudioMixer.SetFloat(floatName, -20);
                VolumeText.text = "Volume: 50%";
            }
            else if (volume == -20f)
            {
                AudioMixer.SetFloat(floatName, -80);
                VolumeText.text = "Volume: 0%";
            }
            else if (volume == -80f)
            {
                AudioMixer.SetFloat(floatName, 0);
                VolumeText.text = "Volume: 100%";
            }
        }
    }
}

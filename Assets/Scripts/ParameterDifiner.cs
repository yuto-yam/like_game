using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 種々の変数を宣言しておくスクリプト
public class ParameterDifiner : MonoBehaviour
{
    // 各種変数の準備
    public int BattleCount = 0; // バトルの数
    public int MazeCount = 0; // 迷路の階層数
    public float Encount_Rate = 0.5f; // 敵の出現頻度 ~1まで
    public bool IsBossBattle = false; // ボス戦を判定するフラグ
    public bool IsFromBattle = false; // バトル→迷路かどうかを制御
    public int MapNumber = 0; // 迷路の番号を記憶
    [SerializeField] Vector2Int cpos = new Vector2Int(1, 1); // 迷路での位置を記憶

    public Vector2Int CPOS // cposはprivateにしたので、値の入出力用の関数を作成
    {
        get { return cpos; }
        set // 迷路外にはいかないようにしておく(最大値は仮の値)
        {
            cpos = new Vector2Int(
                Mathf.Clamp(value.x, 1, 21), // 1～21に補正
                Mathf.Clamp(value.y, 1, 21)  // 1～21に補正
            );
        }
    }

    // キャラクター生成用の基底クラス
    public class Character
    {
        public string Name { get; set; }
        public int Lv { get; set; }
        public int MaxHP { get; set; }
        public int HP { get; set; }
        public int ATK { get; set; }

        // コンストラクタ
        public Character(string name, int lv, int maxHP, int atk)
        {
            Name = name;
            Lv = lv;
            MaxHP = maxHP;
            HP = maxHP;
            ATK = atk;
        }

        public void SetLv(int newLv) => Lv = newLv;
        public void SetMaxHP(int newMaxHP) => MaxHP = newMaxHP;
        public void SetATK(int newATK) => ATK = newATK;

        // ダメージ処理
        public int TakeDamage(int damage)
        {
            int reducedDamage = Mathf.Max(damage - Lv, 0);  // 防御力としてレベルを引く
            HP -= reducedDamage;

            // HPが0未満にならないように
            HP = Mathf.Max(HP, 0);

            UnityEngine.Debug.Log($"{Name} Current HP: {HP} (Damage Taken: {reducedDamage})");
            return reducedDamage;
        }

        // 回復処理
        public int HealDamage(int heal)
        {
            HP = Mathf.Min(HP + heal, MaxHP);  // 最大HPを超えないように
            UnityEngine.Debug.Log($"{Name} Current HP: {HP} (Healed: {heal})");
            return heal;
        }

        // 死亡時の処理（オーバーライドを想定）
        public virtual void YouAreDEAD()
        {
            UnityEngine.Debug.Log($"{Name} is dead.");
        }
    }

    // プレイヤーを生成し、管理するクラス
    public class Player : Character
    {
        // プロパティとしてExpを管理
        public int Exp { get; private set; }

        // コンストラクタで基底クラスを呼び出し、Expも設定
        public Player(string name, int lv, int exp, int maxHP, int atk) : base(name, lv, maxHP, atk)
        {
            Exp = exp;
        }

        // 経験値を加算する
        public void AddExp(int exp) => Exp += exp;

        // レベルアップ時の処理
        public void LevelUP()
        {
            MaxHP += Lv * 2;
            HP = Mathf.Min(HP + Lv * 2, MaxHP);  // HPがMaxHPを超えないように
            ATK += Lv;
            Lv += 1;

            // 経験値リセット
            Exp = 0;
        }

        // プレイヤー死亡時の処理
        public override void YouAreDEAD()
        {
            UnityEngine.Debug.Log($"{Name} (Player) is DEAD.");
        }
    }

    // 敵を生成し、管理するクラス
    public class Enemy : Character
    {
        private ParameterDifiner parameterDifiner;

        public Enemy(string name, int lv, int maxHP, int atk, ParameterDifiner parameterDifiner) : base(name, lv, maxHP, atk)
        {
            this.parameterDifiner = parameterDifiner;
        }

        public override void YouAreDEAD() // 敵が死亡時
        {
            UnityEngine.Debug.Log("Enemy is down.");
            parameterDifiner.IsFromBattle = true;

            if (parameterDifiner.IsBossBattle)
            {
                parameterDifiner.IsBossBattle = false;
                SceneManager.LoadScene("Drop");
            }
            else
            {
                SceneManager.LoadScene("Maze");
            }
        }
    }

    // 武器につけるスキル
    public enum SKILL
    {
        MAGIC, // レベル防御貫通攻撃
        HEAL   // 回復
    }

    // 色を管理するクラス
    public static class ColorPalette
    {
        // 定義する固定パレット
        private static readonly Color[] Palette = new Color[]
        {
            Color.black,
            Color.white,
            Color.grey,
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.cyan,
            Color.magenta
        };

        // Color → 名前
        public static string ColorToName(int index)
        {
            switch (index)
            {
                case 0: return "無垢の";
                case 1: return "闇の";
                case 2: return "灰の";
                case 3: return "赫き";
                case 4: return "草の";
                case 5: return "蒼の";
                case 6: return "空の";
                case 7: return "花の";
                case 8: return "桃の";
                default: return "無地の";
            }
        }

        // Color → インデックス
        public static int ColorToIndex(Color color)
        {
            for (int i = 0; i < Palette.Length; i++)
            {
                if (Mathf.Approximately(Palette[i].r, color.r) &&
                    Mathf.Approximately(Palette[i].g, color.g) &&
                    Mathf.Approximately(Palette[i].b, color.b))
                {
                    return i;
                }
            }
            return -1; // 見つからない場合
        }

        // インデックス → Color
        public static Color IndexToColor(int index)
        {
            if (index >= 0 && index < Palette.Length)
            {
                return Palette[index];
            }
            return Color.clear; // 無効値
        }

        // パレットの総数を取得
        public static int GetTotalColors()
        {
            return Palette.Length;
        }
    }

    // 武器クラス
    public class Weapon
    {
        public string WeaponName { get; private set; }
        public int WeaponATK { get; private set; }
        public SKILL WeaponSkill { get; private set; }
        public int SpriteIndex { get; private set; }
        public int ColorIndex { get; private set; }

        public List<Sprite> WeaponSpriteList; // 武器の画像リスト

        // コンストラクタ
        public Weapon(string wname, int watk, SKILL skill, int spriteIndex, int colorIndex)
        {
            WeaponName = wname;
            WeaponATK = watk;
            WeaponSkill = skill;
            SpriteIndex = spriteIndex;
            ColorIndex = colorIndex;
        }
    }
}

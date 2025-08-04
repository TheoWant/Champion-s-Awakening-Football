using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class CardsGameLoader : MonoBehaviour
{
    string cards_path;

    public List<CardMatch> Cards;

    public async Task LoadCardsAsync()
    {
        cards_path = Path.Combine(Application.persistentDataPath, "Card_Game.csv");

        if (File.Exists(cards_path))
        {
            string[] lines;
            lines = await Task.Run(() =>File.ReadAllLines(cards_path));

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');

                for (int k = 0; k < data.Length; k++)
                {
                    data[k] = data[k].Replace("@", ",");
                }

                string[] s1 = data[4].Split(';');
                string[] s2 = data[5].Split(';');

                string[][] impactsStrings = new string[][]
                {
                s1,
                s2
                };

                int[] impactC1Data = new int[s1.Length];
                int[] impactC2Data = new int[s2.Length];



                for (int j = 0; j < impactsStrings[0].Length; j++)
                {
                    impactC1Data[j] = Convert.ToInt32(impactsStrings[0][j]);
                    impactC2Data[j] = Convert.ToInt32(impactsStrings[1][j]);
                }

                CardMatch cm = new CardMatch
                {
                    _id = data[0],
                    _text = data[1],
                    _choice_A = data[2],
                    _choice_B = data[3],
                    _impactC1 = impactC1Data,
                    _impactC2 = impactC2Data,
                    nextCard_ChoiceA_1 = data[6],
                    nextCard_ChoiceA_2 = data[7],
                    proba_nC_CA1 = Convert.ToInt32(data[8]),
                    nextCard_ChoiceB_1 = data[9],
                    nextCard_ChoiceB_2 = data[10],
                    proba_nC_CB1 = Convert.ToInt32(data[11]),
                    imageFileName = data[12],
                };

                Cards.Add(cm);
            }
        }

        SaveObject save = new SaveObject();
        save.cardMatchList = Cards;

        SaveManagement.Write(save , Path.Combine(Application.persistentDataPath, "CardGame.xml"));

        return;
    }
}

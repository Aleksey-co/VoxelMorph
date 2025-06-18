using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;


public class NumberInput : MonoBehaviour
{
    public TMP_InputField inputField;

    public void ReadNumber()
    {
        if (int.TryParse(inputField.text, out int number))
        {
            if(number < 0){number = number * (-1);}
            if(number < 1){
            	number = 1;
            }
            if(number > 64){
            	number = 64;
            }

            calcNumbers(number);
            inputField.text = number.ToString();;
            VoxelMeshGenerator.changedSize = true;
            GameObject.Find("Main Camera").GetComponent<OBJConverter>().initializeOBJConverter();
        }
        else
        {
            Debug.Log("Ошибка ввода! Введите целое число.");
        }
    }





    public static void calcNumbers(int number1){
        // 1. Находим количество чанков, округляя в большую сторону
        int chk = (int)Math.Ceiling(Math.Sqrt(number1));
        // 2. Определяем размер обычного чанка
        int size = number1 / chk;
        // 3. Проверяем, если остаток блоков больше, чем размер обычного чанка
        if (number1 % chk != 0)
        {
            size = (int)Math.Ceiling((float)number1 / chk); // Перерасчитываем размер чанка
        }
        // 4. Определяем размер последнего чанка
        int lst = number1 - size * (chk - 1);

            OBJConverter.n = number1; //размер модели
            OBJConverter.chunkN = chk; //количество чанков
            OBJConverter.normalChunk = size; //размер обычного чанка
            OBJConverter.lastChunk = lst; //размер последнего чанка
            OBJConverter.modelHistory.Clear();
            OBJConverter.modelHistorySelectionStart.Clear();
            OBJConverter.modelHistorySelectionEnd.Clear();

        // Выводим результаты
        //Debug.Log($"Число: {number1}, Чанков: {chk}, Размер обычного чанка: {size}, Последний чанк: {lst}");
    }





}



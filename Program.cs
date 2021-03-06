﻿/*
 *Лабораторная: 2.1.
 *Источник: Hwmw.Blogspot.com
 *
 *Язык: C Sharp (C#) v7.3.
 *Среда: Microsoft Visual Studio 2019 v16.7.6.
 *Платформа: .NET Framework v4.7.2.
 *API: console.
 *Изменение: 31.10.2020.
 *Защита: 31.10.2020.
 *
 *Задание: разработайте приложение загружающее 32-х битную динамическую библиотеку, получает адрес функции TheFunc библиотеки.
 *     Функция имеет соглашение о вызове cdecl и имеет 2 аргумента: ваша фамилия (указатель на нуль-терминированную строку
 *     ansi символов), значение x - 8 байтное вещественное число. Функция возвращает результат в виде 8 байтного вещественного
 *     числа. Функция имеет вид y = a * x^2 + b * x + c. Нарисуйте график функции в пределах от 0 до 10 и определите
 *     коэффициенты a, b и с и выведите формулу на график. 
 *
 *Примечание:
 *1. Для работы программы требуется установка дополнительных библиотек (USING SYSTEM).
 */

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace SourceNamespace {

internal class MainClass {
     [DllImport("Lib2-1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]

private static extern double TheFunc(IntPtr lastname, double x);

public static void Main () {
     string lastname = "Ivanov" + char.MinValue;                                   //создаём указатель на строку с фамилией
     byte[] bytes = ASCIIEncoding.ASCII.GetBytes(lastname);
     IntPtr lastnamePtr = Marshal.AllocHGlobal(bytes.Length);
     Marshal.Copy(bytes,0,lastnamePtr, bytes.Length);
     DrawChart(lastnamePtr);                                                       //рисуем график
     Marshal.FreeHGlobal(lastnamePtr);                                             //освобождаем память
}

private static double [] GetCoefficients(IntPtr lastnamePtr) {
     double c = TheFunc(lastnamePtr, 0.0d);                                        //коэффициент c получаем, просто подставив x = 0
     double y1 = TheFunc(lastnamePtr, 1.0);                                        //затем получим значения функции при фиксированных x (x=1 и x=2)
     double y2 = TheFunc(lastnamePtr, 2.0);
     double b = (4 * y1 - y2 + 30) / 2.0d;                                         //рассчитываем коэффициент b из системы уравнений
     double a = y1 + 10 - b;                                                       //рассчитываем коэффициент b из системы уравнений
     return new double[] {a, b, c};
}

private static void DrawChart(IntPtr lastnamePtr) {
     double[] coefs = GetCoefficients(lastnamePtr);                               // получаем коэффициенты выражения
     Chart chart = new Chart();                                                   // создаём экземпляр графика
     string equation = coefs[0].ToString() + "x^2 ";                              // формируем строку с видом функции
     equation += coefs[1] == 0 ? "" : coefs[1] < 0 ? coefs[1].ToString() : "+ " + coefs[1].ToString();
     equation += "x ";
     equation += coefs[2] == 0 ? "" : coefs[2] < 0 ? coefs[2].ToString() : "+ " + coefs[2].ToString();
     Series serie = chart.Series.Add(equation);                                    // создаём новую серию данных
     serie.ChartType = SeriesChartType.Spline;                                     // вид графика - линия
     int valuesCount = (int) (10 / 0.1) + 1;                                       // рассчитываем кол-во точек. Шаг - 0.1
     double current_x = 0.0;                                                       // заполняем график данными
     for (int i = 0; i < valuesCount; i++) {
          double val = TheFunc(lastnamePtr, current_x);
          if (i > 90)
               Console.WriteLine(current_x.ToString()+";" +val.ToString());
          serie.Points.AddXY(current_x, val);
          current_x += 0.1d;
     }
     ChartArea ca = new ChartArea();                                               // создаём область графика
     ca.Name = "ChartArea1";
     ca.BackColor = Color.White;
     ca.BorderColor = Color.FromArgb(26, 59, 105);
     ca.BorderWidth = 0;
     ca.BorderDashStyle = ChartDashStyle.Solid;
     ca.AxisX = new Axis();
     ca.AxisY = new Axis();
     chart.ChartAreas.Add(ca);
     chart.Legends.Add(new Legend(equation));                                      // добавляем легенду для отображения найденной функции
     chart.Legends[equation].DockedToChartArea = "ChartArea1";
     chart.Series[equation].Legend = equation;
     chart.Series[equation].IsVisibleInLegend = true;
     chart.DataBind();                                                             // прикрепляем данные к графику
     chart.SaveImage("C:\\Chart.png", System.Drawing.Imaging.ImageFormat.Png);     // сохраняем график в файл
}
}
}
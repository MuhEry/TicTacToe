using System;
using System.Collections.Generic;

class TicTacToe
{
    static char[] tablo = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    static char oyuncu = 'X';
    static char bilgisayar = 'O';
    static Random rastgele = new Random();
    static List<int> bosHucreler = new List<int>();

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("Zorluk seviyesi seçiniz: (1 kolay), (2 Orta seviye), (3 Çok zor)");
            int zorluk = Convert.ToInt16(Console.ReadLine());
            while (zorluk < 1 || zorluk > 3)
            {
                Console.Write("Geçersiz sayı, tekrar deneyin: ");
                zorluk = Convert.ToInt16(Console.ReadLine());
            }
            int turSayisi = 0;
            while (true)
            {
                Console.Clear();
                tabloYazdır();

                if (turSayisi % 2 == 0)
                {
                    oyuncuHamlesi();
                }
                else
                {
                    switch (zorluk) // Zorluk seviyesine göre fonksiyonlar çağırılır
                    {
                        case 3:
                            bilgisayarHamlesi();
                            break;
                        case 2:
                            rastgeleHamle(1); // rastgeleHamle fonksiyonunun aldığı parametre rastgele hamle yapma olasılığıdır
                            break;
                        case 1:
                            rastgeleHamle(2); // Kolay oyun için rastgele hamle yapma ihtimali daha yüksek seçilir
                            break;
                    }
                }

                turSayisi++;
                int oyunSonucu = Kontrol(0); // Oyunun bitme durumunu kontrol et
                if (oyunSonucu != 0)
                {
                    Console.Clear();
                    tabloYazdır();
                    if (oyunSonucu >= 10)
                        Console.WriteLine("Bilgisayar Kazandı!");
                    else if (oyunSonucu == -10)
                        Console.WriteLine("Oyuncu Kazandı!");
                    else
                        Console.WriteLine("Berabere!");
                    break;
                }

                if (turSayisi == 9)  // Tablo dolunca bitir
                {
                    Console.Clear();
                    tabloYazdır();
                    Console.WriteLine("Berabere!");
                    break;
                }
            }
            Console.WriteLine("Tekrar Oynamak için (0) :");
            int tekrar = Convert.ToInt32(Console.ReadLine());
            if (tekrar != 0) break; // 0 girilirse oyun tekrar başlatılır, yoksa program kapanır
            else
            {
                for (int i = 0; i < 9; i++)
                {
                    tablo[i] = (char)('1' + i);
                }
            }
        }
    }

    static void tabloYazdır()
    {
        Console.WriteLine($" {tablo[0]} | {tablo[1]} | {tablo[2]} ");
        Console.WriteLine("---|---|---");
        Console.WriteLine($" {tablo[3]} | {tablo[4]} | {tablo[5]} ");
        Console.WriteLine("---|---|---");
        Console.WriteLine($" {tablo[6]} | {tablo[7]} | {tablo[8]} ");
    }

    static void oyuncuHamlesi()
    {
        Console.Write("Oyuncu hamlesi (1-9): ");
        int hamle = Convert.ToInt32(Console.ReadLine());
        while (hamle < 1 || hamle > 9 || tablo[hamle - 1] == 'X' || tablo[hamle - 1] == 'O')
        {
            Console.Write("Geçersiz hamle, tekrar deneyin: ");
            hamle = Convert.ToInt32(Console.ReadLine());
        }
        tablo[hamle - 1] = oyuncu;
    }

    static void rastgeleHamle(int zorlukSeviyesi)
    {
        bosHucreler.Clear();
        int üretilen = rastgele.Next(1,4); // 1, 2 yada 3 rakamını seçer
        if (üretilen <= zorlukSeviyesi)
        {
            for (int i = 0; i < 9; i++)
            {
                if (tablo[i] != 'X' && tablo[i] != 'O') // Boş hücreler için kontrol sağlar
                {
                    bosHucreler.Add(i); // Boş hücreleri listeye ekler
                }
            }
            tablo[bosHucreler[rastgele.Next(bosHucreler.Count)]] = 'O'; // Boş hücrelerden birini rastgele seçerek hamle yapılır
        }
        else bilgisayarHamlesi(); // İhtimale bağlı olarak en iyi hamle yapılır
    }
    static void bilgisayarHamlesi()
    {
        int maxPuan = int.MinValue; // Bilgisayar için en iyi skoru tutacak
        int iyiHamle = -1; // En iyi hamleyi tutacak

        for (int i = 0; i < 9; i++)
        {
            if (tablo[i] != 'X' && tablo[i] != 'O') // Boş hücreler için kontrol sağlar
            {
                char geciciHamle = tablo[i];
                tablo[i] = bilgisayar; // Geçici olarak bilgisayarın hamlesini simüle et
                int skor = Minimax(tablo, 0, false);
                /* Rekursif Minimax fonksiyonunu çağırarak olası tüm oyun hamlelerinin olasıklarını 
                simüle et ve en iyi sonuç veren skoru al */
                tablo[i] = geciciHamle; // Hamleyi geri al

                if (skor > maxPuan)
                {
                    maxPuan = skor; // En yüksek skoru bul
                    iyiHamle = i; // En iyi hamleyi güncelle
                }
            }
        }
        tablo[iyiHamle] = bilgisayar; // Bilgisayarın hamlesini yap
    }

    static int Minimax(char[] yeniTablo, int hamleDerinliği, bool max)
    {
        int score = Kontrol(hamleDerinliği); // Kazanma durumunu kontrol et
        if (score != 0) // Kazanan varsa skoru döndür
            return score;

        bool tahtaDolu = true;
        foreach (char hucre in yeniTablo)
        {
            if (hucre != 'X' && hucre != 'O')
            {
                tahtaDolu = false;
                break;
            }
        }

        if (tahtaDolu)
            return 0; // Hamle yapacak yer kalmadı, oyun berabere

        if (max) // Sıra bilgisayarda ise bilgisayarın yapabileceği en iyi hamle tüm oyun sonları simüle edilerek bulunur
        {
            int maxSkor = int.MinValue;
            for (int i = 0; i < 9; i++)
            {
                if (yeniTablo[i] != 'X' && yeniTablo[i] != 'O')
                {
                    char geciciHamle = yeniTablo[i];
                    yeniTablo[i] = bilgisayar;
                    maxSkor = Math.Max(maxSkor, Minimax(yeniTablo, hamleDerinliği + 1, false)); // Hamlelerden daha yüksek puanlı olanı bul
                    yeniTablo[i] = geciciHamle;
                }
            }
            return maxSkor;
        }

        /* Oyun sonları simüle edilirken sıra oyuncuda ise oyuncunun bilgisayara en düşük puanı 
        kazandıracak hamleyi bulması sağlanarak simülasyon devam eder */
        else
        {
            int minSkor = int.MaxValue;
            for (int i = 0; i < 9; i++)
            {
                if (yeniTablo[i] != 'X' && yeniTablo[i] != 'O')
                {
                    char geciciHamle = yeniTablo[i];
                    yeniTablo[i] = oyuncu;
                    minSkor = Math.Min(minSkor, Minimax(yeniTablo, hamleDerinliği + 1, true)); // Bilgisayarın aldığı en düşük puanı bul
                    yeniTablo[i] = geciciHamle;
                }
            }
            return minSkor;
        }
    }
    static int Kontrol(int hamle)
    {
        int[][] winPositions = new int[][]
        {
            new int[] {0, 1, 2},
            new int[] {3, 4, 5},
            new int[] {6, 7, 8},
            new int[] {0, 3, 6},
            new int[] {1, 4, 7},
            new int[] {2, 5, 8},
            new int[] {0, 4, 8},
            new int[] {2, 4, 6}
        };

        // Kazananı kontrol et
        foreach (var pos in winPositions)
        {
            if (tablo[pos[0]] == tablo[pos[1]] && tablo[pos[1]] == tablo[pos[2]]) // Eğer üç hücre de aynı işarete sahipse
            {
                if (tablo[pos[0]] == bilgisayar) // Eğer kazanan bilgisayar ise 10, oyuncu ise -10 döndür
                {
                    if(hamle == 0) // Eğer bilgisayar simülasyon sırasında tek hamlede oyunu kazanıyorsa bu hamle en yüksek puanı alır
                    {
                        return int.MaxValue;
                    }
                    return 10;
                }
                else return -10; 
            }
        }
        return 0; // Kazanan yok
    }
}
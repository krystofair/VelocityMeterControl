# VelocityMeterControl

## base ver 0.5
1. Własne wgrywanie tarczy i wskaźnika [x]
2. Ewentualny ekstra element na środku tarczy [=]
3. Kwadratowy kształt kontrolki [x]

## version 1.0
1. Optymalizacja wyświetlania obrazów kontrolki [x]
2. Stały rozmiar wielkości tarczy, aby dopasować punkt startowy wskaźnika [=]
    * Rozmiar obrazu ze wskaźnikiem taki sam jak rozmiar obrazu tarczy, ale
    * Po wczytaniu obrazu wskaźnika i poznaniu jego początkowej pozycji,
    * Usunięcie kanału alpha i pozostawienie tylko obrazu wskaźnika.
    * Ustawienie origin i dalej już taka sama rotacja jak wcześniej.
    [x]
3. Zmniejszenie wymaganych rozmiarów dla wskaźnika oraz obrazu ekstra [=]
4. Zmiana algorytmu ustawiania wskaźnika w odpowiedniej pozycji [x]
    * Rotacja z ustawieniem pivot - osi.

## version 1.2
1. Możliwość ustawienia pierwszej pozycji wskaźnika przez użytkownika [x] **automatycznie spełnione
dzięki pkt 2. z wersji 1.0**
2. Ograniczona wielkość obrazu dla tarczy? [ ] **nie przekraczanie pewnej wielkości minimalnej
i maksymalnej**

## version 2.0
1. Owalny kształt kontrolki i zmiana jej wielkości za pomocą promienia [ ]
2. Thread-safe [ ] - występowanie błędu podczas asynchronicznego dostępu do obrazu wskaźnika,
    w celu obliczenia prostokąta do odrysowania.
3. Podwójne buforowanie obrazu kontrolki [ ]
    Jeżeli rodzic ma tę opcję ustawioną?
    Możliwość włączenie przez properties? - spoko implementacja.
    ```csharp
        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.DoubleBuffer, true);
    ```
## Dodatkowe możliwe optymalizacje / zmiany.
1. Funkcja wyciągania obrazu z przezroczystości określonego stopnia
    do zrobienia tego przez odczytywanie pamięci przez wskaźnika.
    Robi się to przez użycie Scan0.ToPointer(), pointer można traktować
    jako tablicę. Klasa w której używamy wskaźnika (użycie tak jak w C),
    jest oznaczona słówkiem `unsafe`.
2. Korzystaj z funkcji Main.Min dla prostoty kiedy wyznaczasz marginalne punkty w prostokącie.

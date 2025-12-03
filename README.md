# 🎄 Choinka 2026 - Desktop Widget (WPF)

Interaktywny gadżet pulpitowy napisany w C# (WPF), łączący animacje graficzne z logiką biznesową i obsługą multimediów.

## 🚀 Główne Funkcjonalności

### 🎨 Grafika i Animacje
* **System cząsteczek:** Proceduralnie generowany, animowany padający śnieg oraz efekty iskier przy interakcji.
* **Dynamiczne oświetlenie:** Lampki choinkowe z animacją płynnego rozświetlania/gaszenia oraz efekt poświaty (Glow) na gwieździe.
* **Tryb "Wydajności":** Optymalizacja zużycia zasobów poprzez klonowanie i zamrażanie obiektów graficznych (`Freeze`).

### 🖱️ Interakcja (UX)
* **Aktywna Gwiazda (Hotspot):** Najechane kursorem na gwiazdę aktywuje tryb "Zawieja" (przyspieszony śnieg), zapala iluminację choinki i generuje iskry.
* **Drag & Drop:** Możliwość dowolnego przesuwania widgetu po pulpicie (okno bez obramowania).
* **System Życzeń:** Wyświetlanie losowych życzeń po dwukrotnym kliknięciu w choinkę.

### 🔊 Audio
* **Obsługa Playlisty:** Odtwarzacz muzyki z funkcją automatycznego przełączania utworów po ich zakończeniu.
* **Sound Effects:** Odtwarzanie dźwięków reakcji synchronicznie z akcjami użytkownika.
* **Sterowanie:** Możliwość zmiany utworu i wyciszenia z poziomu menu.

### ⚙️ System i Logika
* **Persistence (Zapis stanu):** Aplikacja zapamiętuje ustawienia użytkownika (muzyka, śnieg) po zamknięciu, wykorzystując `Properties.Settings`.
* **System Tray:** Integracja z zasobnikiem systemowym (minimalizacja, obsługa menu kontekstowego).
* **Licznik:** Precyzyjne odliczanie czasu (dni, godziny, minuty) do najbliższego Bożego Narodzenia.

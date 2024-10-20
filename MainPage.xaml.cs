using System;
using Microsoft.Maui.Controls;

namespace TRPO
{
    public partial class MainPage : ContentPage
    {
        private int[,] board = new int[4, 4];
        private bool isGameOver = false;

        public MainPage()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            board = new int[4, 4];
            AddRandomTile();
            AddRandomTile();
            UpdateGrid();
        }

        private void AddRandomTile()
        {
            if (!HasEmptyCells())
            {
                return;
            }

            Random rand = new();
            int x, y;
            do
            {
                x = rand.Next(4);
                y = rand.Next(4);
            } while (board[x, y] != 0);
            board[x, y] = rand.Next(1, 3) * 2; // 2 или 4
        }

        private bool HasEmptyCells()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (board[i, j] == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool HasAvailableMoves()
        {
            // Проверяем на наличие пустых клеток
            if (HasEmptyCells())
            {
                return true;
            }

            // Проверяем возможность слияния блоков
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i < 3 && board[i, j] == board[i + 1, j]) // Проверяем вниз
                    {
                        return true;
                    }
                    if (j < 3 && board[i, j] == board[i, j + 1]) // Проверяем вправо
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void CheckGameOver()
        {
            if (!HasAvailableMoves())
            {
                isGameOver = true;
                DisplayGameOverMessage();
            }
        }

        private void DisplayGameOverMessage()
        {
            // Выводим сообщение о завершении игры
            DisplayAlert("Игра окончена", "Нет доступных ходов", "ОК");
        }


        private void UpdateGrid()
        {
            GameGrid.Children.Clear();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var label = new Label
                    {
                        Text = board[i, j] == 0 ? "" : board[i, j].ToString(),
                        FontSize = 32,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        BackgroundColor = board[i, j] == 0 ? Color.FromRgb(211, 211, 211) : GetTileColor(board[i, j])
                    };
                    Grid.SetRow(label, i);
                    Grid.SetColumn(label, j);
                    GameGrid.Children.Add(label);
                }
            }
            CheckGameOver();
            CheckWinCondition();
        }

        private void CheckWinCondition()
        {
            // Проверяем наличие плитки со значением 2048
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (board[i, j] == 2048)
                    {
                        DisplayWinMessage();
                        return;
                    }
                }
            }
        }

        private void DisplayWinMessage()
        {
            // Выводим сообщение о победе
            DisplayAlert("Поздравляем!", "Вы достигли 2048!", "ОК");
        }

        static private Color GetTileColor(int value)
        {
            return value switch
            {
                2 => Color.FromRgb(238, 228, 218),
                4 => Color.FromRgb(237, 224, 200),
                8 => Color.FromRgb(242, 177, 121),
                16 => Color.FromRgb(245, 149, 99),
                32 => Color.FromRgb(246, 124, 95),
                64 => Color.FromRgb(246, 94, 59),
                128 => Color.FromRgb(237, 207, 114),
                256 => Color.FromRgb(237, 204, 97),
                512 => Color.FromRgb(237, 200, 80),
                1024 => Color.FromRgb(237, 197, 63),
                2048 => Color.FromRgb(237, 194, 46),
                _ => Color.FromRgb(204, 192, 179),
            };
        }

        private void OnLeftClicked(object sender, EventArgs e)
        {
            if (isGameOver) return;

            if (MoveLeft())
            {
                AddRandomTile();
            }
            UpdateGrid();
        }

        private void OnRightClicked(object sender, EventArgs e)
        {
            if (isGameOver) return;

            if (MoveRight())
            {
                AddRandomTile();
            }
            UpdateGrid();
        }

        private void OnUpClicked(object sender, EventArgs e)
        {
            if (isGameOver) return;

            if (MoveUp())
            {
                AddRandomTile();
            }
            UpdateGrid();
        }

        private void OnDownClicked(object sender, EventArgs e)
        {
            if (isGameOver) return;

            if (MoveDown())
            {
                AddRandomTile();
            }
            UpdateGrid();
        }


        private bool MoveLeft()
        {
            bool moved = false;
            for (int i = 0; i < 4; i++)
            {
                int[] row = new int[4];
                int j = 0;

                // Собираем ненулевые элементы в новый массив с начала
                for (int k = 0; k < 4; k++)
                {
                    if (board[i, k] != 0)
                    {
                        row[j++] = board[i, k];
                    }
                }

                // Объединяем одинаковые блоки
                for (int k = 0; k < 3; k++)
                {
                    if (row[k] == row[k + 1] && row[k] != 0)
                    {
                        row[k] *= 2;
                        row[k + 1] = 0;
                        moved = true;
                    }
                }

                // Сдвигаем ненулевые элементы влево после объединения
                int[] newRow = new int[4];
                j = 0;
                for (int k = 0; k < 4; k++)
                {
                    if (row[k] != 0)
                    {
                        newRow[j++] = row[k];
                    }
                }

                // Присваиваем новую строку обратно в board
                for (int k = 0; k < 4; k++)
                {
                    if (board[i, k] != newRow[k])
                    {
                        moved = true;
                    }
                    board[i, k] = newRow[k];
                }
            }
            return moved;
        }


        private bool MoveRight()
        {
            bool moved = false;
            for (int i = 0; i < 4; i++)
            {
                int[] row = new int[4];
                int j = 3;

                // Собираем ненулевые элементы в новый массив с конца
                for (int k = 3; k >= 0; k--)
                {
                    if (board[i, k] != 0)
                    {
                        row[j--] = board[i, k];
                    }
                }

                // Объединяем одинаковые блоки
                for (int k = 3; k > 0; k--)
                {
                    if (row[k] == row[k - 1] && row[k] != 0)
                    {
                        row[k] *= 2;
                        row[k - 1] = 0;
                        moved = true;
                    }
                }

                // Сдвигаем ненулевые элементы вправо после объединения
                int[] newRow = new int[4];
                j = 3;
                for (int k = 3; k >= 0; k--)
                {
                    if (row[k] != 0)
                    {
                        newRow[j--] = row[k];
                    }
                }

                // Присваиваем новую строку обратно в board
                for (int k = 0; k < 4; k++)
                {
                    if (board[i, k] != newRow[k])
                    {
                        moved = true;
                    }
                    board[i, k] = newRow[k];
                }
            }
            return moved;
        }







        private bool MoveUp()
        {
            bool moved = false;
            for (int i = 0; i < 4; i++)
            {
                int[] col = new int[4];
                int j = 0;

                // Собираем ненулевые элементы в новый массив с начала
                for (int k = 0; k < 4; k++)
                {
                    if (board[k, i] != 0)
                    {
                        col[j++] = board[k, i];
                    }
                }

                // Объединяем одинаковые блоки
                for (int k = 0; k < 3; k++)
                {
                    if (col[k] == col[k + 1] && col[k] != 0)
                    {
                        col[k] *= 2;
                        col[k + 1] = 0;
                        moved = true;
                    }
                }

                // Сдвигаем ненулевые элементы вверх после объединения
                int[] newCol = new int[4];
                j = 0;
                for (int k = 0; k < 4; k++)
                {
                    if (col[k] != 0)
                    {
                        newCol[j++] = col[k];
                    }
                }

                // Присваиваем новый столбец обратно в board
                for (int k = 0; k < 4; k++)
                {
                    if (board[k, i] != newCol[k])
                    {
                        moved = true;
                    }
                    board[k, i] = newCol[k];
                }
            }
            return moved;
        }






        private bool MoveDown()
        {
            bool moved = false;
            for (int i = 0; i < 4; i++)
            {
                int[] col = new int[4];
                int j = 3;

                // Собираем ненулевые элементы в новый массив с конца
                for (int k = 3; k >= 0; k--)
                {
                    if (board[k, i] != 0)
                    {
                        col[j--] = board[k, i];
                    }
                }

                // Объединяем одинаковые блоки
                for (int k = 3; k > 0; k--)
                {
                    if (col[k] == col[k - 1] && col[k] != 0)
                    {
                        col[k] *= 2;
                        col[k - 1] = 0;
                        moved = true;
                    }
                }

                // Сдвигаем ненулевые элементы вниз после объединения
                int[] newCol = new int[4];
                j = 3;
                for (int k = 3; k >= 0; k--)
                {
                    if (col[k] != 0)
                    {
                        newCol[j--] = col[k];
                    }
                }

                // Присваиваем новый столбец обратно в board
                for (int k = 0; k < 4; k++)
                {
                    if (board[k, i] != newCol[k])
                    {
                        moved = true;
                    }
                    board[k, i] = newCol[k];
                }
            }
            return moved;
        }






        private void OnRestartClicked(object sender, EventArgs e)
        {
            InitializeGame();
            isGameOver = false;
        }
    }
}
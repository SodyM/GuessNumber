using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GuessNumber
{
    /// <remarks>
    /// Class that encapsulates the board of number tiles to guess
    /// </remarks>
    class NumberBoard
    {
        // game constants
        const int BORDER_SIZE = 8;
        const int NUM_COLUMNS = 3;
        const int NUM_ROWS = NUM_COLUMNS;
        const string BOARD_IMAGE = "board";

        // drawing support
        Texture2D boardTexture;
        Rectangle drawRectangle;

        // side length for each tile
        int tileSideLength;

        // tiles
        NumberTile[,] tiles = new NumberTile[NUM_ROWS, NUM_COLUMNS];

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contentManager">the content manager</param>
        /// <param name="center">the center of the board</param>
        /// <param name="sideLength">the side length for the board</param>
        /// <param name="correctNumber">the correct number</param>
        /// <param name="soundBank">the sound bank for sound effects</param>
        public NumberBoard(ContentManager contentManager, Vector2 center, int sideLength, int correctNumber, SoundBank soundBank)
        {
            // load content for the board and create draw rectangle
            LoadContent(contentManager);
            
            // calculate side length for number tiles
            tileSideLength = (sideLength - ((NUM_COLUMNS + 1) * BORDER_SIZE)) / NUM_COLUMNS;
            
            // set rectangle properties
            drawRectangle = new Rectangle((int)center.X - sideLength / 2, (int)center.Y - sideLength / 2, sideLength, sideLength);

            // initialize array of number tiles
            int number = 0;
            for (int row = 0; row < NUM_COLUMNS; row++)
            {
                for (int column = 0; column < NUM_COLUMNS; column++)
                {
                    number++;
                    tiles[row, column] = new NumberTile(contentManager,  CalculateTileCenter(row, column), 
                                                        tileSideLength, number, correctNumber, soundBank);
                }
            }
        }

        /// <summary>
        /// Updates the board based on the current mouse state. The only required action is to identify
        /// that the left mouse button has been clicked and update the state of the appropriate number
        /// tile.
        /// </summary>
        /// <param name="gameTime">the current GameTime</param>
        /// <param name="mouse">the current mouse state</param>
        /// <return>true if the correct number was guessed, false otherwise</return>
        public bool Update(GameTime gameTime, MouseState mouse)
        {
            // update all the number tiles
            for (int row = 0; row < NUM_COLUMNS; row++)
            {
                for (int column = 0; column < NUM_COLUMNS; column++)
                {
                    bool result = tiles[row, column].Update(gameTime, mouse);
                    if (result)
                        return result;
                }
            }

            // return false because the correct number wasn't guessed
            return false;
        }

        /// <summary>
        /// Draws the board
        /// </summary>
        /// <param name="spriteBatch">the SpriteBatch to use for the drawing</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // draw the board
            spriteBatch.Draw(boardTexture, drawRectangle, Color.White);
            
            // draw all the number tiles     
            for (int row = 0; row < NUM_COLUMNS; row++)
            {
                for (int column = 0; column < NUM_COLUMNS; column++)
                {
                    tiles[row, column].Draw(spriteBatch);
                }
            }
        }

        /// <summary>
        /// Loads the content for the board
        /// </summary>
        /// <param name="contentManager">the content manager</param>
        private void LoadContent(ContentManager contentManager)
        {
            // load the background for the board
            boardTexture = contentManager.Load<Texture2D>(BOARD_IMAGE);
        }

        /// <summary>
        /// Calculates the center of the tile at the given row and column
        /// </summary>
        /// <param name="row">the row in the array</param>
        /// <param name="column">the column in the array</param>
        /// <returns>the center of the tile in the given row and column</returns>
        private Vector2 CalculateTileCenter(int row, int column)
        {
            int upperLeftX = drawRectangle.X + (BORDER_SIZE * (column + 1)) +  tileSideLength * column;
            int upperLeftY = drawRectangle.Y + (BORDER_SIZE * (row + 1)) +  tileSideLength * row;
            return new Vector2(upperLeftX + tileSideLength / 2, upperLeftY + tileSideLength / 2);
        }
    }
}
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GuessNumber
{
    /// <remarks>
    /// A number tile
    /// </remarks>
    class NumberTile
    {
        // game constants
        const string INCORRECT_GUESS = "incorrectGuess";
        const string CORRECT_GUESS = "correctGuess";

        // original length of each side of the tile
        int originalSideLength;

        // whether or not this tile is the correct number
        bool isCorrectNumber;

        // drawing support
        Texture2D texture;
        Rectangle drawRectangle;
        Rectangle sourceRectangle;

        bool tileIsVisible = true;
        bool tileIsBlinking = false;
        bool tileIsSchrinking = false;
        bool clickStarted = false;
        bool buttonReleased = true;

        // shinking support
        const int TOTAL_SHRINK_MILLISECONDS = 4000;
        int elapsedShrinkMilliseconds = 0;

        // blinking support
        const int TOTAL_BLINK_MILLISECONDS = 4000;
        int elapsedBlinkMilliseconds = 0;
        
        const int FRAME_BLINK_MILLISECONDS = 1000;
        int elapsedFrameMilliseconds = 0;

        Texture2D blinkingTexture;
        Texture2D currentTexture;

        // audio support
        SoundBank soundBank;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contentManager">the content manager</param>
        /// <param name="center">the center of the tile</param>
        /// <param name="sideLength">the side length for the tile</param>
        /// <param name="number">the number for the tile</param>
        /// <param name="correctNumber">the correct number</param>
        /// <param name="soundBank">the sound bank for playing cues</param>
        public NumberTile(ContentManager contentManager, Vector2 center, int sideLength, int number, int correctNumber, SoundBank soundBank)
        {
            // set original side length field
            this.originalSideLength = sideLength;

            // set sound bank field
            this.soundBank = soundBank;    

            // load content for the tile and create draw rectangle
            LoadContent(contentManager, number);
            drawRectangle = new Rectangle((int)center.X - sideLength / 2, (int)center.Y - sideLength / 2, sideLength, sideLength);

            // set isCorrectNumber flag
            isCorrectNumber = number == correctNumber;
        }

        /// <summary>
        /// Updates the tile based on game time and mouse state
        /// </summary>
        /// <param name="gameTime">the current GameTime</param>
        /// <param name="mouse">the current mouse state</param>
        /// <return>true if the correct number was guessed, false otherwise</return>
        public bool Update(GameTime gameTime, MouseState mouse)
        {           
            // check if this tile is schrinkig and set correct behaviour
            if (tileIsBlinking)
            {
                #region handle blinking tile
                elapsedBlinkMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedBlinkMilliseconds >= TOTAL_BLINK_MILLISECONDS)
                {
                    return true;
                }
                else 
                {
                    elapsedFrameMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                    if (elapsedFrameMilliseconds >= FRAME_BLINK_MILLISECONDS)
                    {
                        if (sourceRectangle.X == 0)
                            sourceRectangle.X = sourceRectangle.Width;
                        else
                            sourceRectangle.X = 0;

                        elapsedFrameMilliseconds = 0;
                    }
                }
                #endregion
            }
            else if (tileIsSchrinking == true)
            {
                # region handle schrinking tile
                // let's schrink selected tile
                elapsedShrinkMilliseconds += gameTime.ElapsedGameTime.Milliseconds;

                // calculate new sidelength
                float shrinkRatio = (TOTAL_SHRINK_MILLISECONDS - elapsedShrinkMilliseconds) / (float)TOTAL_SHRINK_MILLISECONDS;
                originalSideLength = (int)(this.originalSideLength * shrinkRatio);

                // make our tile smaller
                if (originalSideLength > 0)
                {
                    drawRectangle.Width = originalSideLength;
                    drawRectangle.Height = originalSideLength;
                    tileIsVisible = true;
                }
                else
                {
                    // done, this tile is not visible anymore
                    tileIsVisible = false;
                }
                #endregion
            }
            else
            {
                #region handle rest
                // check if mouse is ove our tile
                if (drawRectangle.Contains(mouse.X, mouse.Y))
                {
                    // set correct sprite (highlight = on)
                    sourceRectangle.X = sourceRectangle.Width;

                    // check for click started on number
                    if (mouse.LeftButton == ButtonState.Pressed && buttonReleased)
                    {
                        clickStarted = true;
                        buttonReleased = false;
                    }
                    else if (mouse.LeftButton == ButtonState.Released)
                    {
                        buttonReleased = true;

                        // if click finished on number, set ui changes
                        if (clickStarted)
                        {
                            if (isCorrectNumber == true)
                            {                                
                                tileIsBlinking = true;
                                soundBank.PlayCue(CORRECT_GUESS);

                                currentTexture = blinkingTexture;
                                sourceRectangle.X = 0;
                                tileIsSchrinking = false;

                                return false;
                            }
                            else
                            {
                                tileIsBlinking = false;
                                tileIsSchrinking = true;
                                soundBank.PlayCue(INCORRECT_GUESS);
                            }
                            clickStarted = false;
                        }
                    }
                }
                else
                {
                    // set correct sprite (highlight = off)
                    sourceRectangle.X = 0;
                }
                #endregion
            }

            // if we get here, return false
            return false;
        }

        /// <summary>
        /// Draws the number tile
        /// </summary>
        /// <param name="spriteBatch">the SpriteBatch to use for the drawing</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // draw the tile
            if (tileIsVisible)
            {
                spriteBatch.Draw(currentTexture, drawRectangle, sourceRectangle, Color.White);
            }
        }

        /// <summary>
        /// Loads the content for the tile
        /// </summary>
        /// <param name="contentManager">the content manager</param>
        /// <param name="number">the tile number</param>
        private void LoadContent(ContentManager contentManager, int number)
        {
            // convert the number to a string
            string numberString = ConvertIntToString(number);

            // load content for the tile and set source rectangle
            texture = contentManager.Load<Texture2D>(numberString);
            sourceRectangle = new Rectangle(0, 0, texture.Width / 2, texture.Height);
            currentTexture = texture;

            // load blinking texture
            string blinkingAsset = "blinking" + numberString;            
            blinkingTexture = contentManager.Load<Texture2D>(blinkingAsset);
        }

        /// <summary>
        /// Converts an integer to a string for the corresponding number
        /// </summary>
        /// <param name="number">the integer to convert</param>
        /// <returns>the string for the corresponding number</returns>
        private string ConvertIntToString(int number)
        {
            switch (number)
            {
                case 1:
                    return "one";
                case 2:
                    return "two";
                case 3:
                    return "three";
                case 4:
                    return "four";
                case 5:
                    return "five";
                case 6:
                    return "six";
                case 7:
                    return "seven";
                case 8:
                    return "eight";
                case 9:
                    return "nine";
                default:
                    throw new Exception("Unsupported number for number tile");
            }
        }
    }
}
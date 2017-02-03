﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EETetris
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Game
	{
		#region Variables
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Texture2D gui;
		Texture2D[] blocks;
		Rectangle gui_rect;
		bool[] keys;
		int x;
		int y;

		bool[,] screen;
		int[,] col;
		bool canSwitch = true;

		bool graphicBeta = true;
		bool[,] block;
		bool[,] heldBlock;
		bool[,,] pieces;
		int[] cols;
		int heldBlockCol = 0;
		int blockCol = 0;

		int counter = 1;
		int limit = 50;
		float countDuration = 0.5f;
		float currentTime = 0f;

		int __using = 0;
		int usingPiece { get { return __using; } set { __using = value; SetBlock(value); x = 5; y = 0; } }
		int held = 0;

		System.Random r;
		#endregion

		public Game1()
			: base()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Switch held and current piece
		/// </summary>
		public void SwitchHeld()
		{
			//If we haven't already switched
			if (canSwitch)
			{
				canSwitch = false;

				//If our held is null, we'll set it to something random
				if (held == -1)
					held = r.Next(0, pieces.GetLength(0));

				int p_tmp = held;

				//Switch held for block
				heldBlock = new bool[4, 4];
				for (int i = 0; i < block.GetLength(0); i++)
					for (int s = 0; s < block.GetLength(1); s++)
						heldBlock[i, s] = block[i, s];
				heldBlockCol = blockCol;

				block = new bool[4, 4];
				blockCol = 0;

				held = usingPiece;
				usingPiece = p_tmp;
			}
		}

		/// <summary>
		/// Increase the Y of the tetris block. Also handles when the block falls down
		/// </summary>
		/// <returns></returns>
		public bool IncreaseY()
		{
			bool go = true;

			for (int i = 0; i < block.GetLength(0); i++)
				for (int s = 0; s < block.GetLength(1); s++)
					if (block[i, s])
					{
						int curX = x + i;
						int curY = y + s;

						if (curY + 1 > 19)
							go = false;
						else if (screen[curX, curY + 1])
							go = false;
					}

			if (go)
				y++;
			else
			{
				canSwitch = true;
				//Set board
				for (int i = 0; i < block.GetLength(0); i++)
					for (int s = 0; s < block.GetLength(1); s++)
						if (block[i, s])
						{
							int curX = x + i;
							int curY = y + s;

							screen[curX, curY] = true;
							col[curX, curY] = blockCol;
						}

				x = 5;
				y = 0;

				//Reset piece type
				SetBlock(r.Next(0, pieces.GetLength(0)));

				bool looking = true;
				while (looking)
				{
					looking = false;

					//Check if there are any full lines
					for (int yp = 0; yp < screen.GetLength(1); yp++)
					{
						bool f = true;
						for (int xp = 0; xp < screen.GetLength(0); xp++)
						{
							if (!screen[xp, yp])
								f = false;
						}

						if (f)
						{
							//Merge the upper layer of the screen downwards
							for (int yx = yp; yx > 0; yx--)
								for (int xp = 0; xp < screen.GetLength(0); xp++)
								{
									screen[xp, yx] = screen[xp, yx - 1];
								}
							looking = true;
						}
					}
				}
			}

			return go;
		}

		/// <summary>
		/// Set the current piece to a different one
		/// </summary>
		/// <param name="piece"></param>
		public void SetBlock(int piece)
		{
			__using = piece;
			blockCol = cols[piece];

			//Set block to false
			for (int i = 0; i < block.GetLength(0); i++)
				for (int s = 0; s < block.GetLength(1); s++)
					block[i, s] = false;

			//Set block to piece
			for (int i = 0; i < pieces.GetLength(1); i++)
				for (int s = 0; s < pieces.GetLength(2); s++)
				{
					block[i, s] = pieces[piece, i, s];
				}
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			#region Random
			//Init random & seeds
			r = new System.Random();
			r.Next(0, 1000);
			#endregion


			// TODO: Add your initialization logic here
			//288 x 352
			graphics.PreferredBackBufferWidth = 288;
			graphics.PreferredBackBufferHeight = 352;
			graphics.ApplyChanges();

			x = 0;
			y = 0;
			held = -1;

			#region Init keys
			keys = new bool[8];
			for (int i = 0; i < keys.Length; i++)
				keys[i] = true;
			#endregion

			#region Init screen blocks
			screen = new bool[10, 20];
			col = new int[10, 20];
			for (int i = 0; i < screen.GetLength(0); i++)
				for (int s = 0; s < screen.GetLength(1); s++)
				{
					screen[i, s] = false;
					col[i, s] = 0;
				}
			#endregion

			#region Init block (the selected blocks)
			block = new bool[4, 4];
			for (int i = 0; i < block.GetLength(0); i++)
				for (int s = 0; s < block.GetLength(1); s++)
					block[i, s] = false;
			#endregion

			#region Init possible tetris pieces
			pieces = new bool[7, 4, 4];
			cols = new int[pieces.GetLength(0)];
			#region Set colors
			for (int n = 0; n < cols.GetLength(0); n++ )
				switch (n)
				{
					case 0:
						cols[n] = 2;
						break;
					case 1:
						cols[n] = 1;
						break;
					case 2:
						cols[n] = 5;
						break;
					case 3:
						cols[n] = 0;
						break;
					case 4:
						cols[n] = 3;
						break;
					case 5:
						cols[n] = 4;
						break;
					case 6:
						cols[n] = 6;
						break;
				}
			#endregion

			#region Set pieces to false
			for (int n = 0; n < pieces.GetLength(0); n++)
				for (int i = 0; i < pieces.GetLength(1); i++)
					for (int s = 0; s < pieces.GetLength(2); s++)
					{
						pieces[n, i, s] = false;
					}
			#endregion

			#region Set pieces
			for (int n = 0; n < pieces.GetLength(0); n++)
				switch (n)
				{
					case 0:
						for (int i = 1; i < 3; i++)
							for (int s = 1; s < 3; s++)
								pieces[n, i, s] = true;
						break;
					case 1:
						pieces[n, 2, 0] = true;
						for (int i = 0; i < 3; i++)
							pieces[n, i, 1] = true;
						break;
					case 2:
						pieces[n, 0, 0] = true;
						for (int i = 0; i < 3; i++)
							pieces[n, i, 1] = true;
						break;
					case 3:
						pieces[n, 1, 0] = true;
						pieces[n, 1, 1] = true;
						pieces[n, 2, 1] = true;
						pieces[n, 2, 2] = true;
						break;
					case 4:
						pieces[n, 0, 1] = true;
						pieces[n, 1, 1] = true;
						pieces[n, 1, 2] = true;
						pieces[n, 2, 2] = true;
						break;
					case 5:
						for (int i = 0; i < 4; i++ )
							pieces[n, 1, i] = true;
						break;
					case 6:
						pieces[n, 2, 1] = true;
						for (int i = 1; i < 4; i++ )
							pieces[n, i, 2] = true;
						break;
				}
			#endregion
			#endregion

			//The original game sets the first block to square
			SetBlock(0);

			base.Initialize();
		}

		/// <summary>
		/// Get a specific snip of a picture
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="wid"></param>
		/// <param name="hei"></param>
		/// <param name="main"></param>
		/// <returns></returns>
		public Texture2D GetSnip(int x, int y, int wid, int hei, Texture2D main)
		{
			//http://gamedev.stackexchange.com/questions/35358/create-a-texture2d-from-larger-image
			Texture2D originalTexture = main;
			Rectangle sourceRectangle = new Rectangle(x, y, wid, hei);

			Texture2D cropTexture = new Texture2D(GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
			Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
			originalTexture.GetData(0, sourceRectangle, data, 0, data.Length);
			cropTexture.SetData(data);
			return cropTexture;
		}

		/// <summary>
		/// Rotate the piece
		/// </summary>
		/// <param name="amt"></param>
		public void Rotate(int amt = 1)
		{
			//Actually rotate x amount of times
			for (int c = 0; c < amt; c++)
			{
				object[,] conv = new object[block.GetLength(0), block.GetLength(1)];
				for (int i = 0; i < conv.GetLength(0); i++)
					for (int n = 0; n < conv.GetLength(1); n++)
						conv[i, n] = (object)block[i, n];

				object[,] back = RotateMatrix(conv, 4);
				for (int i = 0; i < back.GetLength(0); i++)
					for (int n = 0; n < back.GetLength(1); n++)
						block[i, n] = (bool)back[i, n];
			}

			//Make sure blocks aren't colliding after the rotation
			for (int i = 0; i < block.GetLength(0); i++)
				for (int s = 0; s < block.GetLength(1); s++)
					if (block[i, s])
					{
						int curX = x + i;
						int curY = y + s;

						//If the rotation was invalid: The rotation was invalid. Go back to the original way we were
						if ((curX < 0 || curX > 9) || (curY < 0 || curY > 19))
						{
							Rotate(4 - amt);
							return;
						}
						//First we check the bounds, then we'll check the screen position

						if (screen[curX, curY])
						{
							Rotate(4 - amt);
							return;
						}
					}
		}

		//stackoverflow
		public object[,] RotateMatrix(object[,] matrix, int n)
		{
			object[,] ret = new object[n, n];

			for (int i = 0; i < n; ++i)
			{
				for (int j = 0; j < n; ++j)
				{
					ret[i, j] = matrix[n - j - 1, i];
				}
			}

			return ret;
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			//Load the GUI
			gui = Content.Load<Texture2D>("gui");
			gui_rect = new Rectangle(0, 0, gui.Width, gui.Height);

			//Get the blocks
			Texture2D blockUse = Content.Load<Texture2D>("blocks");

			//Snip out the specific amount of required blocks
			blocks = new Texture2D[16];

			for (int y = 0; y < 2; y++)
				for (int x = 0; x < 8; x++)
					blocks[x + (y * 8)] = GetSnip(x * 16, y * 16, 16, 16, blockUse);

			// TODO: use this.Content to load your game content here
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			var k = Keyboard.GetState();

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || k.IsKeyDown(Keys.Escape))
				Exit();

			//KEY PRESS TEMPLATE
			/*
			int num = 7;
			if (k.IsKeyDown(Keys.X)) {
				if(keys[num]) {
					keys[num] = false;
					//key press code
				}
			} else {
				keys[num] = true; 
			}
			 */

			#region Q ( Switch between BETA and BASIC )
			if (k.IsKeyDown(Keys.Q))
			{
				if (keys[6])
				{
					keys[6] = false;

					//Switch everything by 8 to get basic/beta
					if (graphicBeta)
					{
						heldBlockCol += 8;
						blockCol += 8;
					}
					else
					{
						heldBlockCol -= 8;
						blockCol -= 8;
					}

					for (int i = 0; i < cols.Length; i++)
					{
						if (graphicBeta)
							cols[i] = cols[i] + 8;
						else
							cols[i] = cols[i] - 8;
					}

					for (int i = 0; i < col.GetLength(0); i++)
						for (int s = 0; s < col.GetLength(1); s++)
							if (graphicBeta)
								col[i, s] = col[i, s] + 8;
							else
								col[i, s] = col[i, s] - 8;

					graphicBeta = !graphicBeta;
				}
			}
			else keys[6] = true;
			#endregion

			#region W [ ROTATE LEFT]
			if (k.IsKeyDown(Keys.W))
			{
				if (keys[3])
				{
					keys[3] = false;

					Rotate(1);
				}
			}
			else keys[3] = true;
			#endregion

			#region S [ ROTATE RIGHT ]
			if (k.IsKeyDown(Keys.S))
			{
				if (keys[5])
				{
					keys[5] = false;

					Rotate(3);
				}
			}
			else keys[5] = true;
			#endregion

			#region C [ SWITCH HELD TO HOTBAR ]
			if (k.IsKeyDown(Keys.C))
			{
				if (keys[4])
				{
					keys[4] = false;

					SwitchHeld();
				}
			}
			else keys[4] = true;
			#endregion

			if (k.IsKeyDown(Keys.D))
			{
				if (keys[1])
				{
					keys[1] = false;

					bool go = true;

					for (int i = 0; i < block.GetLength(0); i++)
						for (int s = 0; s < block.GetLength(1); s++)
							if (block[i, s])
							{
								int curX = x + i;
								int curY = y + s;

								if (curX + 1 > 9)
									go = false;
								else if (screen[curX + 1, curY])
									go = false;
							}

					if (go)
						x += 1;
				}
			}
			else keys[1] = true;

			if (k.IsKeyDown(Keys.A))
			{
				if (keys[0])
				{
					keys[0] = false;
					bool go = true;

					for (int i = 0; i < block.GetLength(0); i++)
						for (int s = 0; s < block.GetLength(1); s++)
							if (block[i, s])
							{
								int curX = x + i;
								int curY = y + s;

								if (curX - 1 < 0)
									go = false;
								else if (screen[curX - 1, curY])
									go = false;
							}

					if (go)
						x -= 1;
				}
			}
			else keys[0] = true;

			currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update()

			if (currentTime >= countDuration)
			{
				currentTime = 0;
				IncreaseY();
			}

			if (k.IsKeyDown(Keys.Space))
			{
				if (keys[2])
				{
					keys[2] = false;
					while (IncreaseY())
					{
						//Keep increasing untill the tetris piece has fallen and hit the board.
					}
				}
			}
			else keys[2] = true;

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			spriteBatch.Begin();

			spriteBatch.Draw(gui, gui_rect, Color.White);
			for (int i = 0; i < block.GetLength(0); i++)
				for (int s = 0; s < block.GetLength(1); s++)
					if (block[i, s])
						spriteBatch.Draw(blocks[blockCol], new Rectangle(((x + i) * 16) + 16, ((y + s) * 16) + 16, 16, 16), Color.White);
			
			for (int i = 0; i < screen.GetLength(0); i++ )
				for (int s = 0; s < screen.GetLength(1); s++)
				{
					if (screen[i, s])
					{
						spriteBatch.Draw(blocks[col[i, s]], new Rectangle((i * 16) + 16, (s * 16) + 16, 16, 16), Color.White);
					}
				}

			//GUI held 13x17
			if(heldBlock != null)
			for (int i = 0; i < heldBlock.GetLength(0); i++)
				for (int s = 0; s < heldBlock.GetLength(1); s++)
					if (heldBlock[i, s])
						spriteBatch.Draw(blocks[heldBlockCol], new Rectangle((i * 16) + (16 * 13), (s * 16) + (16 * 16), 16, 16), Color.White);

			spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
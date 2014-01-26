using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IndentReplace
{
	public class SpaceToTabConverter
	{
		private const int Space = (int)' ';
		private const int Tab = (int)'\t';
		private const int R = (int)'\r';
		private const int N = (int)'\n';

		private readonly Stream _src;
		private readonly Stream _dst;
		private int _tabSize;

		public SpaceToTabConverter(Stream src, Stream dst, int tabSize)
		{
			_src = src;
			_dst = dst;
			_tabSize = tabSize;
		}

		public void Convert()
		{
			while (ProcessChar());
		}

		private State _state = State.BeginLine;
		private int _cutedSpaces = 0;

		private bool ProcessChar()
		{
			var ch = _src.ReadByte();
			if (ch == -1)
				return false;

			_state = NewState(ch);
			return true;
		}

		private State NewState(int ch)
		{
			switch (_state)
			{
				case State.BeginLine: return BeginLine(ch);
				case State.CutSpace: return CutSpace(ch);
				case State.AnyChar:
				default: return AnyChar(ch);
			}
		}

		private State BeginLine(int ch)
		{
			switch (ch)
			{
				case R:
				case N:
					_dst.WriteByte((byte)ch);
					return State.BeginLine;
				case Space:
					_cutedSpaces++;
					return State.CutSpace;
				default:
					_dst.WriteByte((byte)ch);
					return State.AnyChar;
			}
		}

		private State AnyChar(int ch)
		{
			switch (ch)
			{
				case R:
				case N:
					_dst.WriteByte((byte)ch);
					return State.BeginLine;
				default:
					_dst.WriteByte((byte)ch);
					return State.AnyChar;
			}
		}

		private State CutSpace(int ch)
		{
			switch (ch)
			{
				case Space:
					if (_cutedSpaces < _tabSize - 1)
					{
						_cutedSpaces++;
						return State.CutSpace;
					}
					else
					{
						_cutedSpaces = 0;
						_dst.WriteByte((byte)Tab);
						return State.BeginLine;
					}
				case R:
				case N:
					for (int i = 0; i < _cutedSpaces; i++)
						_dst.WriteByte((byte)Space);
					_dst.WriteByte((byte)ch);
					_cutedSpaces = 0;
					return State.BeginLine;
				default:
					for (int i = 0; i < _cutedSpaces; i++)
						_dst.WriteByte((byte)Space);
					_cutedSpaces = 0;
					_dst.WriteByte((byte)ch);
					return State.AnyChar;
			}
		}

		private enum State
		{
			BeginLine,
			CutSpace,
			AnyChar
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IndentReplace
{
	public class TabToSpaceConverter
	{
		private const int Space = (int)' ';
		private const int Tab = (int)'\t';
		private const int R = (int)'\r';
		private const int N = (int)'\n';

		private readonly Stream _src;
		private readonly Stream _dst;
		private int _tabSize;

		public TabToSpaceConverter(Stream src, Stream dst, int tabSize)
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
				case State.AnyChar:
				default: return AnyChar(ch);
			}
		}

		private State BeginLine(int ch)
		{
			switch (ch)
			{
				case Tab:
					for (int i = 0; i < _tabSize; i++)
						_dst.WriteByte((byte)Space);
					return State.BeginLine;
				case R:
				case N:
					_dst.WriteByte((byte)ch);
					return State.BeginLine;
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

		private enum State
		{
			BeginLine,
			AnyChar
		}
	}
}

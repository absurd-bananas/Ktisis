// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Json.BlockBufferJsonReader
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.IO;

namespace Ktisis.Data.Json;

public ref struct BlockBufferJsonReader(
	Stream stream,
	Span<byte> blockBuffer,
	JsonReaderOptions options
) {
	public Utf8JsonReader Reader = new Utf8JsonReader();
	private readonly Stream stream = stream;
	private readonly Span<byte> blockBuffer = blockBuffer;
	private Span<byte> readSlice = new Span<byte>();
	private JsonReaderState jsonState = new JsonReaderState(options);
	private State state = State.INIT;

	public bool Read() {
		switch (this.state) {
			case State.INIT:
				this.acquireReader();
				goto case State.READING;
			case State.READING:
			case State.FINAL_READ:
				if (((Utf8JsonReader) ref this.Reader).
				this.Read())
				return true;
				if (this.state == State.FINAL_READ) {
					this.state = State.CLOSED;
					return false;
				}
				goto case State.INIT;
			case State.CLOSED:
				return false;
			default:
				throw new Exception("This point is unreachable");
		}
	}

	private void acquireReader() {
		var num1 = 0;
		if (this.state != State.INIT) {
			if (((Utf8JsonReader) ref this.Reader).BytesConsumed == 0L)
			throw new Exception("JSON value appears to exceed the bounds of the block buffer. Increase the buffer size or decrease your JSON value size.");
			this.jsonState = ((Utf8JsonReader) ref this.Reader).CurrentState;
			ref Span<byte> local = ref this.readSlice;
			var bytesConsumed = (int)((Utf8JsonReader) ref this.Reader).BytesConsumed;
			Span<byte> span = local.Slice(bytesConsumed, local.Length - bytesConsumed);
			span.CopyTo(this.blockBuffer);
			num1 = span.Length;
		}
		var stream = this.stream;
		Span<byte> blockBuffer = this.blockBuffer;
		ref Span<byte> local1 = ref blockBuffer;
		var num2 = num1;
		Span<byte> span1 = local1.Slice(num2, local1.Length - num2);
		var num3 = stream.Read(span1);
		this.readSlice = this.blockBuffer.Slice(0, num1 + num3);
		this.state = this.readSlice.Length == 0 ? State.FINAL_READ : State.READING;
		this.Reader = new Utf8JsonReader(Span<byte>.op_Implicit(this.readSlice), this.readSlice.Length == 0, this.jsonState);
	}

	private enum State {
		INIT,
		READING,
		FINAL_READ,
		CLOSED
	}
}

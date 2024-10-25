using System;
using System.Collections.Generic;

public class RoomGenerator
{
    private int maxIterations;
    private int roomLengthMin;
    private int roomWidthMin;
    public RoomGenerator(int maxIterations, int roomLengthMin, int roomWidthMin)
    {
        this.maxIterations = maxIterations;
        this.roomLengthMin = roomLengthMin;
        this.roomWidthMin = roomWidthMin;
    }

    internal List<RoomNode> GenerateRoomInGivenSpace(List<Node> roomSpace)
    {
        throw new NotImplementedException();
    }
}
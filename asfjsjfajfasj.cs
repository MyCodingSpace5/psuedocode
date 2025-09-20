function GreedyMeshing(voxels, size):
    quads = []

    for each axis d in {X, Y, Z}:
        u = (d + 1) % 3   
        v = (d + 2) % 3  
        q = vector(0,0,0); q[d] = 1

        for slice = -1 to size-1:
            mask = 2D array [size][size] of 0s
            for y = 0 to size-1:
                for x = 0 to size-1:
                    a = voxel(s = slice, u = x, v = y) is solid?
                    b = voxel(s = slice+1, u = x, v = y) is solid?
                    if a != b:
                        mask[x][y] = 1   
                    else:
                        mask[x][y] = 0
            rectangles = ExtractRectangles(mask)
            for each rect in rectangles:
                quads.append( MakeQuad(rect, slice, d, u, v, q) )

    return quads
function ExtractRectangles(mask):
    rectangles = []
    for y = 0 to height-1:
        for x = 0 to width-1:
            if mask[x][y] == 1:
                rectWidth  = 1
                rectHeight = 1
                while x + rectWidth < width and mask[x + rectWidth][y] == 1:
                    rectWidth++
                canGrow = true
                while canGrow:
                    for dx = 0 to rectWidth-1:
                        if mask[x+dx][y+rectHeight] != 1:
                            canGrow = false
                            break
                    if canGrow:
                        rectHeight++
                for dy = 0 to rectHeight-1:
                    for dx = 0 to rectWidth-1:
                        mask[x+dx][y+dy] = 0
                rectangles.append( (x, y, rectWidth, rectHeight) )
    return rectangles
function MakeQuad(rect, slice, d, u, v, q):
    (x, y, width, height) = rect
    pos[d] = slice + (q[d] > 0 ? 1 : 0)
    pos[u] = x
    pos[v] = y
    normal = unit vector along axis d * sign(q[d])
    return Quad(pos, width, height, u, v, normal)


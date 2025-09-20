Mesh:
    vertices[]: list of 3D positions
    faces[]: list of triangles (3 vertex indices)
Quadric[v]: 4x4 symmetric error matrix for each vertex
PriorityQueue: edges sorted by collapse cost
function SimplifyMesh_QEM(mesh, targetFaceCount):
    for each vertex v in mesh.vertices:
        Quadric[v] = ZeroMatrix()
    for each face f in mesh.faces:
        plane = ComputePlaneEquation(f)
        for each vertex v in f:
            Quadric[v] += OuterProduct(plane, plane)
    queue = PriorityQueue()
    for each edge (v1, v2) in mesh.edges:
        cost, optimalPos = ComputeCollapseCost(v1, v2)
        queue.insert((v1, v2), cost)
    while mesh.faceCount > targetFaceCount and not queue.empty():
        edge = queue.popLowestCost()
        (v1, v2) = edge
        if edge is invalid (topology issues): continue
        newPos, newQuadric = CollapseEdge(v1, v2)
        update vertex position and quadric
        cleanup faces that became invalid
        for each neighbor u of newVertex:
            newCost, newPos = ComputeCollapseCost(newVertex, u)
            queue.update((newVertex, u), newCost)
    return mesh
function ComputePlaneEquation(face):
    n = normalize( cross(b - a, c - a) )
    d = -dot(n, a)
    return [n.x, n.y, n.z, d]   // plane: ax + by + cz + d = 0
function OuterProduct(p, p):
    return matrix where M[i][j] = p[i]*p[j]
function ComputeCollapseCost(v1, v2):
    Q = Quadric[v1] + Quadric[v2]
    if Q is invertible:
        newPos = Solve(Q[0..2,0..2], -Q[0..2,3])
    else:
        newPos = midpoint(v1, v2)
    cost = newPos^T * Q * newPos
    return (cost, newPos)
function CollapseEdge(v1, v2):
    newQuadric = Quadric[v1] + Quadric[v2]
    newPos = optimal position (from ComputeCollapseCost)
    return (newPos, newQuadric)

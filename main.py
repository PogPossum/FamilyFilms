from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
import pypyodbc as odbc

app = FastAPI(title="Family Films API")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"], 
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

CONNECTION_STRING = (
    "DRIVER={ODBC Driver 18 for SQL Server};"
    "SERVER=192.168.0.52;" 
    "DATABASE=FamilyFilms;"
    "UID=sa;"
    "PWD=Password1;"
    "TrustServerCertificate=yes;"
    "Encrypt=no;" 
)

def get_db_connection():
    try:
        return odbc.connect(CONNECTION_STRING)
    except Exception as e:
        print(f"Database connection failed: {e}")
        return None

# API Endpoint: Fetch all family movies
@app.get("/api/movies")
def get_movies():
    conn = get_db_connection()
    if not conn:
        raise HTTPException(status_code=500, detail="Database connection error")
    
    cursor = conn.cursor()
    
    query = """
        SELECT 
            MovieID, 
            RTRIM(Title) as Title, 
            Release, 
            RTRIM(Animated) as Animated, 
            RTRIM(Location) as Location, 
            RTRIM(Studio) as Studio
        FROM Movies
        ORDER BY Title ASC
    """
    
    try:
        cursor.execute(query)
        columns = [column[0] for column in cursor.description]
        rows = cursor.fetchall()
        
        results = []
        for row in rows:
            results.append(dict(zip(columns, row)))
            
        return results
        
    except Exception as e:
        print(f"Query execution failed: {e}")
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        cursor.close()
        conn.close()
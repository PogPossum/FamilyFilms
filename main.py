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
    "SERVER=x.x.x.x;" 
    "DATABASE=FilmsDB;"
    "UID=user;"
    "PWD=Password123;"
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
    
    # Query raw columns directly without RTRIM expressions in the SELECT clause
    query = """
        SELECT MovieID, Title, Release, Location, Category, Studio
        FROM Movies
        ORDER BY Title ASC
    """
    
    try:
        cursor.execute(query)
        rows = cursor.fetchall()
        
        results = []
        for row in rows:
            # Map index positions directly and handle string stripping safely in Python
            results.append({
                "MovieID": row[0],
                "Title": str(row[1]).strip() if row[1] else "",
                "Release": row[2],
                "Location": str(row[3]).strip() if row[3] else "",
                "Category": str(row[4]).strip() if row[4] else "",
                "Studio": str(row[5]).strip() if row[5] else ""
            })
            
        return results
        
    except Exception as e:
        print(f"Query execution failed: {e}")
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        cursor.close()
        conn.close()

from fastapi import FastAPI
from pydantic import BaseModel
import spacy

nlp = spacy.load("en_core_web_sm")
app = FastAPI()


class TextRequest(BaseModel):
    text: str


@app.post("/analyze")
async def analyze(request: TextRequest):
    doc = nlp(request.text)
    text_lower = request.text.lower()

    intent = "unknown"
    if any(w in text_lower for w in ["find", "search", "book", "read", "книга", "знайти"]):
        intent = "book_search"
    elif any(w in text_lower for w in ["weather", "temperature", "погода", "градусів"]):
        intent = "weather_request"

    entities = [{"text": ent.text, "label": ent.label_} for ent in doc.ents]

    return {"intent": intent, "entities": entities}


if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="0.0.0.0", port=8000)
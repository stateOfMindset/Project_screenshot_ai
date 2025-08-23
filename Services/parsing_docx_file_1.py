from docx import Document
import json
import os
import  re

def parse_docx(file_path):
    doc = Document(file_path)
    qa_pairs = []
    for para in doc.paragraphs:
        text = para.text.strip()
        text = text.replace('מילה נרדפת' , '')
        re.sub(r'[^א-ת\s]','',text)
        if ":" in text:
            # Split at last colon
            parts = text.rsplit(":", 1)

            question = parts[0]
            question = question.replace(":", "").strip()

            answer = parts[1]
            answer = answer.replace(":", "").strip()
            print({"question": question, "answer": answer})
            qa_pairs.append({"question": question, "answer": answer})
    return qa_pairs


# Collect from multiple files
all_qa = []

files_in_dir = os.listdir(os.path.join(os.getcwd(), "Data","docs"))
for file in files_in_dir:
    if file.endswith(".docx"):
        all_qa.extend(parse_docx(os.path.join(os.path.join(os.getcwd(), "Data","docs"), file)))

# Save as JSON
with open("stage_2.json", "w", encoding="utf-8") as f:
    json.dump(all_qa, f, ensure_ascii=False, indent=2)

print("✅ Extracted", len(all_qa), "questions")

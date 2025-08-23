from docx import Document
import re, json , os

def count_words_hebrew(text: str) -> int:
    # Remove Hebrew nikud (vowel marks)
    no_nikud = re.sub(r'[\u0591-\u05C7]', '', text)
    # Remove punctuation (keep letters, numbers, whitespace)
    no_nikud = re.sub(r'[^\w\s\u0590-\u05FF]', ' ', no_nikud)
    tokens = [t for t in no_nikud.strip().split() if t]
    return len(tokens)

def treat_substring(text: str) -> str:
    treated = text.replace(":" , "")
    return treated.strip()

def extract_multiword_qa(file, out_json_path: str = None):
    doc = file
    qa_dict = {}

    for para in doc.paragraphs:
        line = para.text.strip()
        if not line:
            continue

        # Remove prefix "מילה נרדפת:"
        line = line.replace("מילה נרדפת" , "")
        line = line.replace("מילה נרדפת ל", "")
        line = line.replace("מילה נרדפתל-", "")
        line = line.replace("מילה נרדפתל", "")
        line = line.replace("מילה נרפת", "")
        line = line.replace("מילה נרדפת ל-", "")
        line = re.sub(r'^\s*מילה\s*נרדפת\s*:\s*', '', line)

        # Split on the first colon only (question:answer)
        parts = line.split(':', 1)
        if len(parts) != 2:
            continue

        question, answer = parts[0].strip(), parts[1].strip()

        question = treat_substring(question)
        answer = treat_substring(answer)


        # Keep only questions that have 2+ words
        if count_words_hebrew(question) >= 2:
            sub_question = question.split(' ')
            if sub_question[0] not in qa_dict:   # keep first occurrence

                qa_dict[sub_question[0]] = question
                #print(qa_dict[question])


    # Save to JSON if path provided
    if out_json_path:
        with open(out_json_path, 'w', encoding='utf-8') as f:
            json.dump(qa_dict, f, ensure_ascii=False, indent=2)

    return qa_dict


# Example usage
if __name__ == "__main__":
    docs_dir = os.path.join(os.getcwd(),"Data","docs")
    for file in os.listdir(docs_dir):
        filePath = os.path.join(docs_dir , file)
        doc = Document(filePath)
        d = extract_multiword_qa(doc , os.path.join(os.getcwd(),"Data","stage_2_upgraded.json"))
        print(d)
//using System.IO;
//using System.Collections.Generic;
//using System.Linq;
//using Index;
//using System;

//namespace SPIMI //single-pass in-memory indexing
//{
//    public class TermVector
//    {
//        Dictionary<string, int> vector;

//        public TermVector()
//        {
//            this.vector = new Dictionary<string, int>();
//        }

//        public ICollection<string> Terms
//        {
//            get
//            {
//                return this.vector.Keys;
//            }
//        }

//        public int GetDimensionLength(string termDimension)
//        {
//            int value;
//            vector.TryGetValue(termDimension, out value);
//            return value;
//        }

//        public TermVector(Dictionary<string, int> vector)
//        {
//            this.vector = vector;
//        }

//        public void AddTerm(string term)
//        {
//            int count = this.GetDimensionLength(term);
//            vector.Remove(term);
//            vector.Add(term, count + 1);
//        }

//        public double CosineSimilarity(TermVector centroid)
//        {
//            return this.DotProduct(centroid) / (this.EuclideanLength() * centroid.EuclideanLength());

//        }

//        public int DotProduct(TermVector other)
//        {
//            int sum = 0;
//            foreach (string term in this.Terms)
//                sum += this.GetDimensionLength(term) * other.GetDimensionLength(term);
//            return sum;
//        }

//        public double EuclideanLength()
//        {
//            double sum = 0;
//            foreach (int length in vector.Values)
//                sum += length * length;
//            return Math.Sqrt(sum);
//        }

//        public static TermVector operator -(TermVector left, TermVector right)
//        {
//            Dictionary<string, int> difference = new Dictionary<string, int>(left.vector);
//            foreach (string term in right.Terms)
//            {
//                int length = 0;
//                if (difference.TryGetValue(term, out length))
//                    difference.Remove(term);
//                difference.Add(term, length - right.GetDimensionLength(term));
//            }

//            return new TermVector(difference);
//        }

//        public static TermVector operator +(TermVector left, TermVector right)
//        {
//            Dictionary<string, int> sum = new Dictionary<string, int>(left.vector);
//            foreach (string term in right.Terms)
//            {
//                int length = 0;
//                if (sum.TryGetValue(term, out length))
//                    sum.Remove(term);
//                sum.Add(term, length + right.GetDimensionLength(term));
//            }

//            return new TermVector(sum);
//        }

//        public IEnumerable<string> GetLengthSortedDimensions()
//        {
//            return this.vector.OrderBy(pair => pair.Value).Select(p => p.Key);
//        }

//        public IEnumerable<string> GetNonZeroDimensions()
//        {
//            return this.vector.Select(p => p.Key);
//        }

//        public static TermVector GetCentroid(IEnumerable<TermVector> vectors)
//        {
//            Dictionary<string, long> sum = new Dictionary<string, long>();

//            int vectorCount = 0;
//            // Sum the lengths of dimensions
//            foreach (TermVector vector in vectors)
//            {
//                vectorCount++;
//                foreach (string term in vector.Terms)
//                {
//                    long count = 0;
//                    sum.TryGetValue(term, out count);
//                    sum.Remove(term);
//                    sum.Add(term, count + vector.GetDimensionLength(term));
//                }
//            }

//            // Divide the dimensions
//            Dictionary<string, int> centroid = new Dictionary<string, int>();
//            foreach (KeyValuePair<string, long> dimension in sum)
//            {
//                centroid.Add(dimension.Key, (int)(dimension.Value / vectorCount));
//            }

//            return new TermVector(centroid);
//        }
//    }
//    public class Posting
//    {
//        public long DocumentId { get; set; }
//        public int Frequency { get; set; }

//        public Posting(long documentId, int frequency)
//        {
//            this.DocumentId = documentId;
//            this.Frequency = frequency;
//        }

//        public override bool Equals(object obj)
//        {
//            Posting other = obj as Posting;
//            return other != null
//                && other.DocumentId.Equals(this.DocumentId)
//                && other.Frequency.Equals(this.Frequency);
//        }

//        public override string ToString()
//        {
//            return "Posting{DocumentId=" + this.DocumentId + ", Frequency=" + this.Frequency + ">";
//        }
//        class MemoryIndex : IIndex<string, IList<Posting>>
//        {
//        // Term -> <DocId ->  (DocId, count)>
//        SortedList<string, SortedList<long, Posting>> index = new SortedList<string, SortedList<long, Posting>>();

//        public int Postings { get; private set; }

//        public SortedList<string, SortedList<long, Posting>> Entries
//        {
//            get
//            {
//                return index;
//            }
//        }

//        public MemoryIndex()
//        {
//            this.Postings = 0;
//        }

//        public void AddTerm(string term, long docId)
//        {
//            // Get the posting list
//            SortedList<long, Posting> postingList;
//            // If we encounter a term for the first time
//            if (!index.TryGetValue(term, out postingList))
//            {
//                postingList = new SortedList<long, Posting>();
//                index.Add(term, postingList);
//            }

//            // Get the posting
//            Posting posting;
//            // If we encounter a docId for the first time
//            if (!postingList.TryGetValue(docId, out posting))
//            {
//                posting = new Posting(docId, 0);
//                postingList.Add(docId, posting);
//            }

//            posting.Frequency += 1;

//            this.Postings++;
//        }

//        public bool TryGet(string key, out IList<Posting> value)
//        {
//            value = null;
//            SortedList<long, Posting> postings = null;
//            if (!index.TryGetValue(key, out postings))
//            {
//                return false;
//            }
//            return true;
//        }
//    }

//    class CollectionMetadataWriter : IDisposable
//    {
//        const int HeaderSize = sizeof(long);
//        FileIndexWriter<long, DocumentInfo> documentsInfo;
//        Stream stream;
//        long collectionTokenCount = 0;
//        public CollectionMetadataWriter(Stream stream)
//        {
//            this.stream = stream;
//            this.stream.Seek(HeaderSize, SeekOrigin.Begin);

//            this.documentsInfo = new FileIndexWriter<long, DocumentInfo>(
//                new LongEncoder(),
//                new DocumentInfoEncoder(), stream);
//        }

//        public void WriteOut()
//        {
//            documentsInfo.WriteOut();

//            this.stream.Seek(0, SeekOrigin.Begin);
//            BinaryWriter writer = new BinaryWriter(this.stream);
//            writer.Write((Int64)collectionTokenCount);
//            this.stream.Seek(0, SeekOrigin.Begin);
//        }

//        internal void AddDocumentInfo(long documentId, DocumentInfo documentInfo)
//        {
//            documentsInfo.Add(documentId, documentInfo);
//            collectionTokenCount += documentInfo.Length;
//        }
//        public void Dispose()
//        {
//            documentsInfo.Dispose();
//        }
//    }

//         class FileIndexWriter<K, V> : IDisposable where K : IComparable<K> 
//    {
//        IBinaryObjectEncoder<K> keyEncoder;
//        IBinaryObjectEncoder<V> valueEncoder;

//        Stream index;
//        Stream data;

//        BinaryWriter indexWriter;
//        BinaryWriter dataWriter;

//        long keyCount = 0;

//        K previousKey;

//        string dataFilePath;

//        long startPosition;

//        long indexPtr;

//        bool isClean = true;
        
//        public bool IsOpen
//        {
//            get
//            {
//                return index.CanWrite && data.CanWrite;
//            }
//        }

//        public FileIndexWriter(
//            IBinaryObjectEncoder<K> keyEncoder, 
//            IBinaryObjectEncoder<V> valueEncoder,
//            Stream index)
//        {
//            Contract.Requires(index != null && index.CanWrite);
//            Contract.Requires(keyEncoder != null);
//            Contract.Requires(valueEncoder != null);
//            Contract.Ensures(this.IsOpen);
//            Contract.Ensures(Contract.OldValue(index.Position) == index.Position);

//            this.keyEncoder = keyEncoder;
//            this.valueEncoder = valueEncoder;
//            this.startPosition = index.Position;
//            this.index = index;
//            this.dataFilePath = Path.GetTempFileName();
//            this.data = File.Open(dataFilePath, FileMode.Open);
            
//            indexWriter = new BinaryWriter(index);
//            dataWriter = new BinaryWriter(data);

//            indexPtr = startPosition + sizeof(Int64);
//        }

//        // Finalizes the index and moves it to filepath
//        public void WriteOut()
//        {
//            //Contract.Requires(this.IsOpen);
//            //Contract.Ensures(this.index.Position == this.startPosition);

//            // Write the key count at the beginning of the index stream
//            index.Seek(startPosition, SeekOrigin.Begin);
//            indexWriter.Write((Int64)keyCount);

//            // Offset all pointers
//            BinaryReader indexReader = new BinaryReader(index);
//            long offset = index.Position + keyCount * sizeof(Int64);
//            for (int i = 0; i < keyCount; i++)
//            {
//                long ptr = indexReader.ReadInt64() + offset;
//                index.Seek(-sizeof(Int64), SeekOrigin.Current);
//                indexWriter.Write((Int64)ptr);
//            }

//            // Copy the postingLists buffer to the termEntries
//            data.Seek(0, SeekOrigin.Begin);
//            data.CopyTo(index);

//            index.Seek(this.startPosition, SeekOrigin.Begin);

//            this.Dispose();
//        }

//        public void Add(K key, V value)
//        {
//            Contract.Requires(this.IsOpen);
//            Contract.Requires(isClean || previousKey.CompareTo(key) < 0, 
//                "Keys must be provided in ascending order");
//            Contract.Requires(key != null);
//            Contract.Ensures(Contract.OldValue(this.index.Position) == this.index.Position,
//                "Stream position is restored");
//            Contract.Ensures(!isClean);
//            Contract.Ensures(Contract.OldValue(this.keyCount) + 1 == this.keyCount);
//            Contract.Ensures(this.previousKey.Equals(key));

//            // 2. Write the pointer to the data in the index
//            long indexStreamPosition = this.index.Position;
//            this.index.Seek(indexPtr, SeekOrigin.Begin);
//            this.indexWriter.Write(data.Position);
//            indexPtr = this.index.Position;

//            // 3. Write out the value
//            this.keyEncoder.write(dataWriter, key);
//            this.valueEncoder.write(dataWriter, value);
            
//            this.keyCount++;
//            this.previousKey = key;
//            isClean = false;
//            this.index.Seek(indexStreamPosition, SeekOrigin.Begin);
//        }

//        public void Dispose()
//        {
//            this.data.Close();
//            if (File.Exists(this.dataFilePath))
//                File.Delete(this.dataFilePath);
//        }
//    }
//    public class SPIMIIndexer
//    {
//        static int maxBlock = 10000;
//        ILexer lexer;
//        IParser parser;
//        List<string> termIndexBlockFilePaths = new List<string>();
//        List<string> documentIndexBlockFilePaths = new List<string>();
//        SpimiBlockWriter termIndexBlockWriter;

//        //CollectionMetadataWriter metadataWriter;

//        long collectionLengthInTokens = 0;

//        long nextDocumentId = 0;

//        Stream indexStream;

//        public void SpimiIndexer(ILexer lexer, IParser parser, Stream indexStream, Stream metadata)
//        {
//            this.lexer = lexer;
//            this.parser = parser;
//            this.termIndexBlockWriter = new SpimiBlockWriter();
//            this.indexStream = indexStream;
//            //this.metadataWriter = new CollectionMetadataWriter(metadata);
//        }
        
//        public void Index(string uri, Stream file)
//        {
//            // Each file holds many documents: we need to parse them out first.
//            foreach (Document document in parser.ExtractDocuments(file))
//            {
//                // Extract the terms from the document and add the document to their respective postings lists
//                long docId = nextDocumentId++;
//                int termsInDoc = 0;
//                TermVector vector = new TermVector();
//                foreach (string term in lexer.Tokenize(document.Body))
//                {
//                    vector.AddTerm(term);

//                    termIndexBlockWriter.AddPosting(term, docId);
//                    if (termIndexBlockWriter.Postings == maxPostingCountPerBlock)
//                    {
//                        this.FlushBlockWriter();
//                    }
//                    termsInDoc++;
//                    collectionLengthInTokens++;
//                }

//                this.metadataWriter.AddDocumentInfo(docId,
//                    new DocumentInfo(uri, document.Title, termsInDoc, document.SpecialIdentifier, vector));
//            }
//        }

//        private void FlushBlockWriter()
//        {
//            string blockFilePath = termIndexBlockWriter.FlushToFile();
//            termIndexBlockFilePaths.Add(blockFilePath);
//        }

//        public void WriteOut()
//        {
//            MergeBlocks();
//            this.metadataWriter.WriteOut();
//        }

//        private void MergeBlocks()
//        {
//            if (termIndexBlockWriter.Postings > 0)
//                FlushBlockWriter();
//            using (FileIndexWriter<string, IList<Posting>> writer = new FileIndexWriter<string, IList<Posting>>(
//                new StringEncoder(),
//                new PostingListEncoder(), indexStream))
//            {
//                SpimiBlockReader blockReader = new SpimiBlockReader();
//                List<IEnumerator<PostingList>> openedBlocks = blockReader.OpenBlocks(this.termIndexBlockFilePaths);
//                foreach (PostingList postingList in blockReader.BeginBlockMerge(openedBlocks))
//                {
//                    writer.Add(postingList.Term, postingList.Postings);
//                }
//                writer.WriteOut();
//            }
//        }
//    }
//    class SpimiBlockWriter
//    {
//        MemoryIndex index = new MemoryIndex();

//        public int Postings
//        {
//            get
//            {
//                return index.Postings;
//            }
//        }

//        public void AddPosting(string term, long docId)
//        {
//            index.AddTerm(term, docId);
//        }

//        public string FlushToFile()
//        {

//            string filename = Path.GetTempFileName();

//            SortedList<string, SortedList<long, Posting>> orderedEntries = index.Entries;

//            using (FileStream fs = File.Open(filename, FileMode.Append))
//            {
//                PostingListEncoder encoder = new PostingListEncoder();
//                BinaryWriter writer = new BinaryWriter(fs);

//                writer.Write((Int32)orderedEntries.Count);

//                foreach (KeyValuePair<string, SortedList<long, Posting>> termPostingsPair in orderedEntries)
//                {
//                    writer.Write((string)termPostingsPair.Key);
//                    encoder.write(writer, termPostingsPair.Value.Values);
//                }
//            }

//            this.index = new MemoryIndex();
//            return filename;
//        }
//    }
//    class SpimiBlockReader
//    {
//        /// <remarks> Keeps file open until all postings have been read</remarks>
//        public IEnumerable<PostingList> Read(string filepath)
//        {
//            using (FileStream stream = File.Open(filepath, FileMode.Open))
//            {
//                PostingListEncoder decoder = new PostingListEncoder();
//                BinaryReader reader = new BinaryReader(stream);
//                int termCount = reader.ReadInt32();
//                for (int termIndex = 0; termIndex < termCount; termIndex++)
//                {
//                    string term = reader.ReadString();
//                    yield return new PostingList(term, decoder.read(reader));
//                }
//            }
//        }

//        public IEnumerable<PostingList> BeginBlockMerge(List<IEnumerator<PostingList>> postingListEnums)
//        {
//            foreach (IEnumerator<PostingList> postingLists in postingListEnums)
//            {
//                postingLists.MoveNext();
//            }

//            while (postingListEnums.Count != 0)
//            {
//                // There can be multiple minimum posting lists, if a term is split into multiple posting lists
//                List<IEnumerator<PostingList>> minimums = new List<IEnumerator<PostingList>>();

//                foreach (IEnumerator<PostingList> postingLists in postingListEnums)
//                {
//                    if (minimums.Count == 0)
//                    {
//                        minimums.Add(postingLists);
//                    }
//                    else
//                    {
//                        string term = minimums[0].Current.Term;
//                        int compareResult = term.CompareTo(postingLists.Current.Term);

//                        if (compareResult == 0)
//                        {
//                            minimums.Add(postingLists);
//                        }
//                        else if (compareResult > 0)
//                        {
//                            minimums.Clear();
//                            minimums.Add(postingLists);
//                        }
//                    }
//                }

//                // Return the next posting list
//                if (minimums.Count > 1)
//                {
//                    // Merge posting lists
//                    string term = minimums[0].Current.Term;

//                    // DocumentId -> Posting
//                    Dictionary<long, Posting> mergedPostingList = new Dictionary<long, Posting>();

//                    foreach (IEnumerator<PostingList> postingListEnum in minimums)
//                    {
//                        PostingList postingList = postingListEnum.Current;
//                        foreach (Posting posting in postingList.Postings)
//                        {
//                            Posting previousPosting;
//                            if (!mergedPostingList.TryGetValue(posting.DocumentId, out previousPosting))
//                            {
//                                mergedPostingList.Add(posting.DocumentId, posting);
//                            }
//                            else
//                            {
//                                previousPosting.Frequency += posting.Frequency;
//                            }
//                        }
//                    }
//                    yield return new PostingList(term, mergedPostingList.Values.ToList());
//                }
//                else
//                {
//                    yield return minimums[0].Current;
//                }

//                // Advance blocks and remove posting empty ones
//                foreach (IEnumerator<PostingList> enumerator in minimums)
//                {
//                    if (!enumerator.MoveNext())
//                    {
//                        postingListEnums.Remove(enumerator);
//                    }
//                }
//            }
//        }

//        public List<IEnumerator<PostingList>> OpenBlocks(IEnumerable<string> blockFilePaths)
//        {
//            List<IEnumerator<PostingList>> postingLists = new List<IEnumerator<PostingList>>();
//            foreach (string filePath in blockFilePaths)
//            {
//                postingLists.Add(Read(filePath).GetEnumerator());
//            }
//            return postingLists;
//        }
//    }
//}
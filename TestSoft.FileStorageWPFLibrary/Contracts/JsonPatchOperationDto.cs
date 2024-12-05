namespace TestSoft.FileStorageWPFLibrary.Contracts
{
    public class JsonPatchOperationDto
    {
        /// <summary>
        /// ��� ��������: add, replace, remove.
        /// </summary>
        public required string Op { get; set; }

        /// <summary>
        /// ���� � �������� � JSON, ��������: /property/nestedProperty.
        /// </summary>
        public required string Path { get; set; }

        /// <summary>
        /// ������, ������� ����������� ��� ���������� (������������ ��� add/replace).
        /// </summary>
        public object? Value { get; set; }
    }

}

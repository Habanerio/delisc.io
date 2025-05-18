export async function useFetchTags({
   count = 50,
   tags = [],
}: {
   count?: number | null;
   tags?: string[] | null;
}) {
   // Default values
   const queryParams = new URLSearchParams();
   queryParams.append('count', count?.toString() || '50');

   if (tags && tags.length > 0) {
      // Only append tags if it's not empty
      queryParams.append('tags', tags.join(',') || '');
   }

   const response = await fetch(`/api/links/tags?${queryParams.toString()}`);

   if (!response.ok) {
      throw new Error('Failed to fetch tags');
   }

   return response.json();
}

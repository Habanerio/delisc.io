/**
 * Get tags as an array, optionally sorted.
 * Will filter out any empty strings and trim whitespace.
 * @param tags - The tags to process
 * @param sorted - Whether to sort the tags
 * @returns An array of tags
 */
export function useGetTagsAsArray(
   tags: string | string[] | undefined,
   sorted: boolean,
): string[] {
   if (!tags) {
      return [];
   }

   let newTags: string[] = [];

   // If tags is a string, split it by commas
   if (typeof tags === 'string') {
      newTags = tags
         .split(',')
         .map((tag) => tag.trim())
         .filter((tag) => tag);
   } else {
      // If tags is an array, filter out any empty strings
      newTags = tags.filter((tag) => tag).map((tag) => tag.trim());
   }

   if (sorted) {
      newTags.sort((a, b) =>
         a.localeCompare(b, undefined, { sensitivity: 'base' }),
      );
   }

   return newTags;
}
